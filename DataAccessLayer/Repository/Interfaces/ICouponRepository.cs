using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface ICouponRepository
{
    // Retrieves coupon by code as CouponViewModel, ensuring it's active
    Task<CouponViewModel> GetCouponByCodeAsync(string code);
    // Records coupon usage in CouponUsage table
    Task<bool> RecordUsageAsync(CouponUsage usage);
    // Gets total usage count for a coupon
    Task<int> GetUsageCountAsync(int couponId);
    // Checks if a user has used a specific coupon
    Task<bool> HasUserUsedCouponAsync(int userId, int couponId);
    // Checks if a user has placed any orders
    Task<bool> HasUserPlacedOrderAsync(int userId);
    Task<List<Coupon>> GetAvailableCoupons();
}
