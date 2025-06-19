using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using System.Linq;

namespace BusinessLogicLayer.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository) => _orderRepository = orderRepository;

    public PaginationViewModel<OrderViewModel> GetOrderList(int pageNumber, string search, int pageSize, string sortColumn, string sortDirection)
    {
        var query = _orderRepository.GetAllOrders();

        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower().Trim();
            query = query.Where(order => order.CustomerName.ToLower().Contains(lowerSearchTerm) || 
                                       order.ItemName.ToLower().Contains(lowerSearchTerm));
        }

        int totalCount = query.Count();

        switch (sortColumn)
        {
            case "ID":
                query = sortDirection == "asc" ? query.OrderBy(o => o.OrderId) : query.OrderByDescending(o => o.OrderId);
                break;
            case "CustomerName":
                query = sortDirection == "asc" ? query.OrderBy(o => o.CustomerName) : query.OrderByDescending(o => o.CustomerName);
                break;
            case "ItemName":
                query = sortDirection == "asc" ? query.OrderBy(o => o.ItemName) : query.OrderByDescending(o => o.ItemName);
                break;
            case "OrderAmount":
                query = sortDirection == "asc" ? query.OrderBy(o => o.OrderAmount) : query.OrderByDescending(o => o.OrderAmount);
                break;
        }

        List<OrderViewModel> orders = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginationViewModel<OrderViewModel>(orders, totalCount, pageNumber, pageSize);
    }

    public OrderViewModel GetOrderById(int orderId) => _orderRepository.GetOrderById(orderId);

    public bool SaveOrder(OrderViewModel orderVM, int userId) => _orderRepository.SaveOrder(orderVM, userId);

    public bool DeleteOrder(int orderId, int userId) => _orderRepository.DeleteOrder(orderId, userId);

    public bool CheckOrderExists(OrderViewModel orderVM) => _orderRepository.CheckOrderExists(orderVM);

    public List<ItemViewModel> GetItemsForAutocomplete(string searchTerm) => _orderRepository.GetItemsForAutocomplete(searchTerm);
}