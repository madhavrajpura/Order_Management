using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IOrderService
{
    Task<bool> CreateOrderAsync(int userId, OrderViewModel orderViewModel);
    Task<List<OrderViewModel>> GetUserOrdersAsync(int userId);
    Task<OrderViewModel> GetOrderById(int OrderId);
    Task<bool> MarkOrderStatus(int orderId);
    Task<PaginationViewModel<OrderViewModel>> GetOrderList(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5, string Status = "");
    // Task<OrderViewModel> GetOrderByIdAsync(int orderId, int userId);
}