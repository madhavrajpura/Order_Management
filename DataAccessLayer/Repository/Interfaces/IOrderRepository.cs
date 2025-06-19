using DataAccessLayer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.Interfaces;

public interface IOrderRepository
{
    IQueryable<OrderViewModel> GetAllOrders();
    OrderViewModel GetOrderById(int orderId);
    bool SaveOrder(OrderViewModel orderVM, int userId);
    bool DeleteOrder(int orderId, int userId);
    bool CheckOrderExists(OrderViewModel orderVM);
    List<ItemViewModel> GetItemsForAutocomplete(string searchTerm);
}