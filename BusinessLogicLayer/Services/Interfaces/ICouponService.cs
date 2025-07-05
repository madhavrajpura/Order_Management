using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Services.Interfaces;

public interface ICouponService
{
    Task<List<CouponViewModel>> GetAvailableCouponsAsync(int userId, decimal cartSubtotal);
    Task<(bool IsValid, string ErrorMessage, CouponViewModel Coupon, decimal Discount)> ValidateCouponAsync(string code, int userId, decimal cartSubtotal);
    Task RecordCouponUsageAsync(int couponId, int userId, int orderId);

}
