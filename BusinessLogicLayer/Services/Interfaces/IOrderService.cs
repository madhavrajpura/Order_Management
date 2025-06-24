using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IOrderService
{
    Task<bool> CreateOrderAsync(int userId, OrderViewModel orderViewModel);
    Task<bool> CreateOrderFromItemAsync(int userId, int itemId, int quantity);
    Task<List<OrderViewModel>> GetUserOrdersAsync(int userId);
    Task<OrderViewModel> GetOrderById(int OrderId);
    Task<bool> MarkOrderStatus(int orderId);
    Task<PaginationViewModel<OrderViewModel>> GetOrderList(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5, string Status = "", int userId = 0);
    // Task<OrderViewModel> GetOrderByIdAsync(int orderId, int userId);
}