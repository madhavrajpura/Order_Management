using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public bool IsDelete { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int? DeletedBy { get; set; }

    public string? Details { get; set; }

    public int Stock { get; set; }

    public virtual ICollection<Cart> Carts { get; } = new List<Cart>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual ICollection<ItemImage> ItemImages { get; } = new List<ItemImage>();

    public virtual ICollection<Notification> Notifications { get; } = new List<Notification>();

    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<WishList> WishLists { get; } = new List<WishList>();
}
