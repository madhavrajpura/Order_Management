using DataAccessLayer.Context;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _db;
    private readonly ICartRepository _cartRepo;

    public OrderRepository(ApplicationDbContext db, ICartRepository cartRepo)
    {
        _db = db;
        _cartRepo = cartRepo;
    }

    public async Task<bool> CreateOrderAsync(Order order, List<OrderItem> orderItems)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var orderItem in orderItems)
            {
                orderItem.OrderId = order.Id;
                _db.OrderItems.Add(orderItem);
                await _db.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error in CreateOrderAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<List<Order>> GetUserOrdersAsync(int userId)
    {
        try
        {
            var data = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Items)
                .Where(o => o.CreatedBy == userId && !o.IsDelete)
                .OrderByDescending(o => o.Id)
                .ToListAsync();
            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserOrdersAsync: {ex.Message}");
            throw;
        }
    }

    public IQueryable<OrderViewModel> GetOrderList()
    {
        try
        {
            var data = _db.Orders.Include(o => o.CreatedByUser)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Items)
                .Where(o => !o.IsDelete)
                .Select(order => new OrderViewModel
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    IsDelivered = order.IsDelivered,
                    CreatedByUser = order.CreatedBy,
                    CustomerName = order.CreatedByUser.Username
                })
                .OrderByDescending(o => o.OrderId) ?? Enumerable.Empty<OrderViewModel>().AsQueryable();

            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserOrdersAsync: {ex.Message}");
            throw;
        }
    }

    public IQueryable<Order> GetOrderListByModel()
    {
        try
        {
            var data = _db.Orders.Include(o => o.CreatedByUser).ThenInclude(o => o.Role)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Items)
                .Where(o => !o.IsDelete && (o.CreatedByUser.RoleId == 3))
                .OrderByDescending(o => o.Id) ?? Enumerable.Empty<Order>().AsQueryable();

            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserOrdersAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> MarkOrderStatus(int orderId)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(order => order.Id == orderId);
        if (order == null) return false;
        order.IsDelivered = true;
        order.UpdatedAt = DateTime.Now;
        order.DeliveryDate = DateTime.Now;
        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
        return true;
    }

}