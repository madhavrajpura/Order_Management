using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface IOrderRepository
{
    Task<bool> CreateOrderAsync(Order order, List<OrderItem> orderItems);
    Task<List<Order>> GetUserOrdersAsync(int userId);
    IQueryable<OrderViewModel> GetOrderList();
    IQueryable<Order> GetOrderListByModel();
    Task<bool> MarkOrderStatus(int orderId);

}