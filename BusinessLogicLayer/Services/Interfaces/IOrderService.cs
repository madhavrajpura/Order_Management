using DataAccessLayer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IOrderService
{
    PaginationViewModel<OrderViewModel> GetOrderList(int pageNumber, string search, int pageSize, string sortColumn, string sortDirection);
    OrderViewModel GetOrderById(int orderId);
    bool SaveOrder(OrderViewModel orderVM, int userId);
    bool DeleteOrder(int orderId, int userId);
    bool CheckOrderExists(OrderViewModel orderVM);
    List<ItemViewModel> GetItemsForAutocomplete(string searchTerm);
}