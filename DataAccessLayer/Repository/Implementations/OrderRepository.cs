using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly NewItemOrderDbContext _db;
    private readonly ICartRepository _cartRepo;

    public OrderRepository(NewItemOrderDbContext db, ICartRepository cartRepo)
    {
        _db = db;
        _cartRepo = cartRepo;
    }

    // Modified: Added stock update logic
    public async Task<bool> CreateOrderAsync(Order order, List<OrderItem> orderItems, List<OrderItemViewModel> orderItemViewModels)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // Added: Update stock for each item
            foreach (var orderItemVM in orderItemViewModels)
            {
                Item item = await _db.Items.FirstOrDefaultAsync(i => i.Id == orderItemVM.ItemId) 
                    ?? throw new Exception($"{orderItemVM.ItemName} not found");
                if (item.Stock < orderItemVM.Quantity)
                    throw new Exception($"Only {item.Stock} units of {orderItemVM.ItemName} available in stock");
                item.Stock -= orderItemVM.Quantity;
                _db.Items.Update(item);
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (OrderItem orderItem in orderItems)
            {
                orderItem.OrderId = order.Id;
                _db.OrderItems.Add(orderItem);
            }
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Order>> GetUserOrdersAsync(int userId)
    {
        List<Order>? data = await _db.Orders
           .Include(o => o.OrderItems)
           .ThenInclude(oi => oi.Item)
           .Where(o => o.CreatedBy == userId && !o.IsDelete)
           .OrderByDescending(o => o.Id)
           .ToListAsync() ?? null;
        return data!;
    }

    public IQueryable<OrderViewModel> GetOrderList()
    {
        IQueryable<OrderViewModel>? data = _db.Orders.Include(o => o.CreatedByNavigation)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
            .Where(o => !o.IsDelete)
            .Select(order => new OrderViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                IsDelivered = order.IsDelivered,
                CreatedByUser = order.CreatedBy,
                CustomerName = order.CreatedByNavigation.Username
            })
            .OrderByDescending(o => o.OrderId) ?? Enumerable.Empty<OrderViewModel>().AsQueryable();

        return data;
    }

    public IQueryable<Order> GetOrderListByModel()
    {
        IQueryable<Order>? data = _db.Orders.Include(o => o.CreatedByNavigation).ThenInclude(o => o.Role)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
            .Where(o => !o.IsDelete && (o.CreatedByNavigation.RoleId == 3))
            .OrderByDescending(o => o.Id) ?? Enumerable.Empty<Order>().AsQueryable();

        return data;
    }

    public async Task<bool> UpdateOrderStatus(int orderId, int UserId)
    {
        Order? order = await _db.Orders.FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new Exception();
        order.IsDelivered = true;
        order.UpdatedAt = DateTime.Now;
        order.DeliveryDate = DateTime.Now;
        order.UpdatedBy = UserId;
        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
        return true;
    }

}