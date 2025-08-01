using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IOrderService
{
    Task<bool> CreateOrderAsync(int userId, OrderViewModel orderViewModel, bool BuyNow);
    Task<List<OrderViewModel>> GetUserOrdersAsync(int userId);
    OrderViewModel GetOrderDetailById(int OrderId);
    Task<bool> UpdateOrderStatus(int orderId, int UserId);
    PaginationViewModel<OrderViewModel> GetOrderList(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5, string Status = "", int userId = 0, string fromDate = "", string toDate = "");
    Task<byte[]> ExportData(string search = "", string Status = "", int UserId = 0, string fromDate = "", string toDate = "");
    // Newwwwwwwwwwwwwwwwwwwwww
    Task<List<OrderViewModel>> GetOrdersByUser(int userId);
    Task<OrderViewModel> GetOrderItems(int orderId);

}