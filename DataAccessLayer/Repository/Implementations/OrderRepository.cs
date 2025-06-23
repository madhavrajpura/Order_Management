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

    public OrderRepository(ApplicationDbContext db,ICartRepository cartRepo)
    {
        _db = db;
        _cartRepo = cartRepo;
    }

    public async Task<bool> CreateOrderAsync(Order order)
    {
        using var transaction  = _db.Database.BeginTransaction();
        try
        {
            _db.Orders.Add(order);
            _db.SaveChanges();

            foreach(var OrderItems in order.OrderItems){
                OrderItems.OrderId = order.Id;
                _db.OrderItems.Add(OrderItems);
            }

            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
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


    
}
