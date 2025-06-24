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
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserOrdersAsync: {ex.Message}");
            throw;
        }
    }

    // public async Task<Order> GetOrderByIdAsync(int orderId, int userId)
    // {
    //     try
    //     {
    //         var data = await _db.Orders
    //             .Include(o => o.OrderItems)
    //             .ThenInclude(oi => oi.Items)
    //             .FirstOrDefaultAsync(o => o.Id == orderId && o.CreatedBy == userId && !o.IsDelete);
    //         return data;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error in GetOrderByIdAsync: {ex.Message}");
    //         throw;
    //     }
    // }

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
                })
                .OrderByDescending(o => o.OrderDate) ?? Enumerable.Empty<OrderViewModel>().AsQueryable();

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
        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
        return true;
    }

}
