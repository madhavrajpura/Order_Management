namespace DataAccessLayer.ViewModels;

public class CouponViewModel
{
    public int CouponId { get; set; }
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderValue { get; set; }
    public decimal? MaxOrderValue { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsFirstOrderOnly { get; set; }
    public bool IsSingleUsePerUser { get; set; }
    public int? TotalUsageLimit { get; set; }
    public bool? IsActive { get; set; }
}