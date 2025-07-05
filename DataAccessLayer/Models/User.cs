using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? LogoutAt { get; set; }

    public int RoleId { get; set; }

    public bool IsDelete { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int? DeletedBy { get; set; }

    public long PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Cart> Carts { get; } = new List<Cart>();

    public virtual ICollection<CouponUsage> CouponUsages { get; } = new List<CouponUsage>();

    public virtual ICollection<Coupon> Coupons { get; } = new List<Coupon>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? DeletedByNavigation { get; set; }

    public virtual ICollection<User> InverseCreatedByNavigation { get; } = new List<User>();

    public virtual ICollection<User> InverseDeletedByNavigation { get; } = new List<User>();

    public virtual ICollection<User> InverseUpdatedByNavigation { get; } = new List<User>();

    public virtual ICollection<Item> ItemCreatedByNavigations { get; } = new List<Item>();

    public virtual ICollection<Item> ItemDeletedByNavigations { get; } = new List<Item>();

    public virtual ICollection<ItemImage> ItemImageCreatedByNavigations { get; } = new List<ItemImage>();

    public virtual ICollection<ItemImage> ItemImageUpdatedByNavigations { get; } = new List<ItemImage>();

    public virtual ICollection<Item> ItemUpdatedByNavigations { get; } = new List<Item>();

    public virtual ICollection<Notification> Notifications { get; } = new List<Notification>();

    public virtual ICollection<Order> OrderCreatedByNavigations { get; } = new List<Order>();

    public virtual ICollection<Order> OrderDeletedByNavigations { get; } = new List<Order>();

    public virtual ICollection<OrderItem> OrderItemCreatedByNavigations { get; } = new List<OrderItem>();

    public virtual ICollection<OrderItem> OrderItemDeletedByNavigations { get; } = new List<OrderItem>();

    public virtual ICollection<OrderItem> OrderItemUpdatedByNavigations { get; } = new List<OrderItem>();

    public virtual ICollection<Order> OrderUpdatedByNavigations { get; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<UserNotification> UserNotifications { get; } = new List<UserNotification>();

    public virtual ICollection<WishList> WishLists { get; } = new List<WishList>();
}
