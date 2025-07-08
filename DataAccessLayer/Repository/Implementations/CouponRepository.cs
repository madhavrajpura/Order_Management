using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class CouponRepository : ICouponRepository
{
    private readonly NewItemOrderDbContext _db;

    public CouponRepository(NewItemOrderDbContext db)
    {
        _db = db;
    }

    public async Task<CouponViewModel> GetCouponByCodeAsync(string code)
    {
        return await _db.Coupons
            .Where(c => c.Code == code && c.IsActive == true)
            .Select(c => new CouponViewModel
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
                IsCombinable = c.IsCombinable,
                TotalUsageLimit = c.TotalUsageLimit,
                IsActive = c.IsActive
            })
            .FirstOrDefaultAsync();
    }

    // public async Task<bool> RecordUsageAsync(CouponUsage usage)
    // {
    //     _db.CouponUsages.Add(usage);
    //     await _db.SaveChangesAsync();
    //     return true;
    // }

    // Counts total usages of a coupon
    public async Task<int> GetUsageCountAsync(int couponId)
    {
        return await _db.CouponUsages
            .CountAsync(cu => cu.CouponId == couponId);
    }

    // Checks if a user has used a specific coupon
    public async Task<bool> HasUserUsedCouponAsync(int userId, int couponId)
    {
        return await _db.CouponUsages
            .AnyAsync(cu => cu.UserId == userId && cu.CouponId == couponId);
    }

    // Checks if a user has placed any orders
    public async Task<bool> HasUserPlacedOrderAsync(int userId)
    {
        return await _db.Orders
            .AnyAsync(o => o.CreatedBy == userId);
    }

    public async Task<List<Coupon>> GetAvailableCoupons()
    {
        return await _db.Coupons.Where(c => (bool)c.IsActive && (!c.ExpiryDate.HasValue || c.ExpiryDate > DateTime.Now)).ToListAsync();
    }
    
}
