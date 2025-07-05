using System.Text.Json;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BusinessLogicLayer.Services.Implementations;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _couponRepository;

    public CouponService(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<List<CouponViewModel>> GetAvailableCouponsAsync(int userId, decimal cartSubtotal)
    {
        var coupons = await _couponRepository.GetAvailableCoupons();
        var couponVMs = coupons.Select(c => new CouponViewModel
        {
            CouponId = c.Id,
            Code = c.Code,
            DiscountType = c.DiscountType,
            DiscountValue = c.DiscountValue,
            MinOrderValue = c.MinOrderValue,
            MaxOrderValue = c.MaxOrderValue,
            ExpiryDate = c.ExpiryDate,
            IsFirstOrderOnly = c.IsFirstOrderOnly,
            IsSingleUsePerUser = c.IsSingleUsePerUser,
            TotalUsageLimit = c.TotalUsageLimit,
            IsActive = c.IsActive
        }).ToList();

        var validCoupons = new List<CouponViewModel>();
        foreach (var coupon in couponVMs)
        {
            (bool isValid, string _, CouponViewModel _, decimal _) = await ValidateCouponAsync(coupon.Code, userId, cartSubtotal);
            if (isValid)
            {
                validCoupons.Add(coupon);
            }
        }

        return validCoupons;
    }

    public async Task<(bool IsValid, string ErrorMessage, CouponViewModel Coupon, decimal Discount)> ValidateCouponAsync(string code, int userId, decimal cartSubtotal)
    {
        var coupon = await _couponRepository.GetCouponByCodeAsync(code);
        if (coupon == null)
            return (false, "Invalid or inactive coupon code", null, 0);

        if (coupon.ExpiryDate.HasValue && DateTime.Now > coupon.ExpiryDate)
            return (false, "Coupon has expired", null, 0);

        if (coupon.MinOrderValue.HasValue && cartSubtotal < coupon.MinOrderValue)
            return (false, $"Minimum order value of ₹{coupon.MinOrderValue} required", null, 0);

        if (coupon.MaxOrderValue.HasValue && cartSubtotal > coupon.MaxOrderValue)
            return (false, $"Maximum order value of ₹{coupon.MaxOrderValue} allowed", null, 0);

        if (coupon.IsFirstOrderOnly && await _couponRepository.HasUserPlacedOrderAsync(userId))
            return (false, "Coupon valid for first orders only", null, 0);

        if (coupon.IsSingleUsePerUser && await _couponRepository.HasUserUsedCouponAsync(userId, coupon.CouponId))
            return (false, "Coupon can only be used once per user", null, 0);

        if (coupon.TotalUsageLimit.HasValue)
        {
            var usageCount = await _couponRepository.GetUsageCountAsync(coupon.CouponId);
            if (usageCount >= coupon.TotalUsageLimit)
                return (false, "Coupon usage limit reached", null, 0);
        }

        decimal discount = coupon.DiscountType == "Percentage"
            ? Math.Min(cartSubtotal, cartSubtotal * (coupon.DiscountValue / 100))
            : Math.Min(coupon.DiscountValue, cartSubtotal);

        return (true, null, coupon, discount);
    }

    public async Task RecordCouponUsageAsync(int couponId, int userId, int orderId)
    {
        await _couponRepository.RecordUsageAsync(new DataAccessLayer.Models.CouponUsage
        {
            CouponId = couponId,
            UserId = userId,
            OrderId = orderId,
            UsedAt = DateTime.Now
        });
    }
}
