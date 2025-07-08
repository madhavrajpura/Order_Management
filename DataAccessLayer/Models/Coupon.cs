using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Coupon
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public decimal? MinOrderValue { get; set; }

    public decimal? MaxOrderValue { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsFirstOrderOnly { get; set; }

    public bool IsSingleUsePerUser { get; set; }

    public int? TotalUsageLimit { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public bool IsCombinable { get; set; }

    public virtual ICollection<CouponUsage> CouponUsages { get; } = new List<CouponUsage>();

    public virtual User CreatedByNavigation { get; set; } = null!;
}
