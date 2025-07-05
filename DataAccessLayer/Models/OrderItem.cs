using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public bool IsDelete { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int? DeletedBy { get; set; }

    public int? OrderId { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
