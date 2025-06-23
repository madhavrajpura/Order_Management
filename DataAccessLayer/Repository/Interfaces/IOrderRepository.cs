using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface IOrderRepository
{
Task<bool> CreateOrderAsync(Order order);    
Task<List<Order>> GetUserOrdersAsync(int userId);
// Task<Order> GetOrderByIdAsync(int orderId, int userId);

}