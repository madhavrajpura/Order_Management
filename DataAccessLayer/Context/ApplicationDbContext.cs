using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }
    public DbSet<ItemImages> ItemImages { get; set; }
    public DbSet<WishList> WishLists { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasKey(table => new { table.Id });
        modelBuilder.Entity<Role>().HasKey(table => new { table.Id });
        modelBuilder.Entity<Item>().HasKey(table => new { table.Id });
        modelBuilder.Entity<Notification>().HasKey(table => new { table.Id });
        modelBuilder.Entity<UserNotification>().HasKey(table => new { table.Id });
        modelBuilder.Entity<ItemImages>().HasKey(table => new { table.Id });
        modelBuilder.Entity<WishList>().HasKey(table => new { table.Id });
        modelBuilder.Entity<Cart>().HasKey(table => new { table.Id });
        modelBuilder.Entity<Order>().HasKey(table => new { table.Id });
        modelBuilder.Entity<OrderItem>().HasKey(table => new { table.Id });

        // modelBuilder.Entity<User>()
        //     .HasOne(u => u.Role)
        //     .WithMany(r => r.Users)
        //     .HasForeignKey(u => u.RoleId)
        //     .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Item>()
            .HasOne(i => i.CreatedByUser)
            .WithMany(u => u.Items)
            .HasForeignKey(i => i.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Item>()
            .HasOne(i => i.UpdatedByUser)
            .WithMany()
            .HasForeignKey(i => i.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<Item>()
            .HasOne(i => i.DeletedByUser)
            .WithMany()
            .HasForeignKey(i => i.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.CreatedByUser)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.UpdatedByUser)
            .WithMany()
            .HasForeignKey(o => o.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.DeletedByUser)
            .WithMany()
            .HasForeignKey(o => o.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(o => o.Orders)
            .HasForeignKey(o => o.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.Orders)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(o => o.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.Items)
            .WithMany()
            .HasForeignKey(o => o.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.CreatedByUser)
            .WithMany()
            .HasForeignKey(o => o.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.UpdatedByUser)
            .WithMany()
            .HasForeignKey(o => o.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.DeletedByUser)
            .WithMany()
            .HasForeignKey(o => o.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
       
        // modelBuilder.Entity<ItemImages>()
        // .HasOne(o => o.CreatedByUser)
        // .WithMany(u => u.Items)
        // .HasForeignKey(o => o.CreatedBy)
        // .OnDelete(DeleteBehavior.Restrict);

        // modelBuilder.Entity<ItemImages>()
        //     .HasOne(o => o.UpdatedByUser)
        //     .WithMany()
        //     .HasForeignKey(o => o.UpdatedBy)
        //     .OnDelete(DeleteBehavior.Restrict)
        //     .IsRequired(false);

        modelBuilder.Entity<User>().Property(p => p.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");
        modelBuilder.Entity<User>().Property(p => p.UpdatedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<User>().Property(p => p.DeletedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<User>().Property(p => p.IsDelete).HasDefaultValue(false);

        modelBuilder.Entity<Item>().Property(p => p.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");
        modelBuilder.Entity<Item>().Property(p => p.UpdatedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Item>().Property(p => p.DeletedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Item>().Property(p => p.IsDelete).HasDefaultValue(false);

        modelBuilder.Entity<Order>().Property(p => p.OrderDate).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");
        modelBuilder.Entity<Order>().Property(p => p.UpdatedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Order>().Property(p => p.DeletedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Order>().Property(p => p.DeliveryDate).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Order>().Property(p => p.IsDelete).HasDefaultValue(false);
        modelBuilder.Entity<Order>().Property(p => p.IsDelivered).HasDefaultValue(false);

        modelBuilder.Entity<OrderItem>().Property(p => p.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");
        modelBuilder.Entity<OrderItem>().Property(p => p.UpdatedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<OrderItem>().Property(p => p.DeletedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<OrderItem>().Property(p => p.IsDelete).HasDefaultValue(false);

        modelBuilder.Entity<Notification>().Property(p => p.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");
        modelBuilder.Entity<Notification>().Property(p => p.IsActive).HasDefaultValue(true);

        modelBuilder.Entity<UserNotification>().Property(p => p.ReadAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<UserNotification>().Property(p => p.IsRead).HasDefaultValue(true);

        modelBuilder.Entity<ItemImages>().Property(p => p.CreatedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<ItemImages>().Property(p => p.UpdatedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<ItemImages>().Property(p => p.IsThumbnail).HasDefaultValue(false);

        modelBuilder.Entity<WishList>().Property(p => p.LikedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Cart>().Property(p => p.CreatedAt).HasColumnType("timestamp without time zone");
    }
}