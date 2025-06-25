using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface IOrderRepository
{
    Task<bool> CreateOrderAsync(Order order, List<OrderItem> orderItems);
    Task<List<Order>> GetUserOrdersAsync(int userId);
    IQueryable<OrderViewModel> GetOrderList();
    Task<bool> MarkOrderStatus(int orderId);
    IQueryable<Order> GetOrderListByModel();

}