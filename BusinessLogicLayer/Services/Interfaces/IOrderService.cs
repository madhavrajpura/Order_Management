using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IOrderService
{
    Task<bool> CreateOrderAsync(int userId, OrderViewModel orderViewModel);
    Task<List<OrderViewModel>> GetUserOrdersAsync(int userId); 
    // Task<OrderViewModel> GetOrderByIdAsync(int orderId, int userId);
}