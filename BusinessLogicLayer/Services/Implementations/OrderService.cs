using System.Linq;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly ICartRepository _cartRepo;

    public OrderService(IOrderRepository orderRepo, ICartRepository cartRepo)
    {
        _orderRepo = orderRepo;
        _cartRepo = cartRepo;
    }

    public async Task<bool> CreateOrderAsync(int userId, OrderViewModel orderViewModel)
    {
        try
        {
            var order = new Order
            {
                TotalAmount = orderViewModel.TotalAmount,
                OrderDate = DateTime.Now,
                CreatedBy = userId
            };

            var orderItemsList = new List<OrderItem>();

            foreach (var orderItemViewModel in orderViewModel.OrderItems)
            {
                OrderItem newOrderItem = new OrderItem
                {
                    ItemId = orderItemViewModel.ItemId,
                    ItemName = orderItemViewModel.ItemName,
                    Price = orderItemViewModel.Price,
                    Quantity = orderItemViewModel.Quantity,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                orderItemsList.Add(newOrderItem);
            }

            var result = await _orderRepo.CreateOrderAsync(order, orderItemsList);

            if (result)
            {
                // Clear cart after successfully Placed order
                var cartItems = await _cartRepo.GetCartItems(userId);
                foreach (var cartItem in cartItems)
                {
                    await _cartRepo.RemoveFromCart(cartItem.Id, userId);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateOrderAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<List<OrderViewModel>> GetUserOrdersAsync(int userId)
    {
        var orders = await _orderRepo.GetUserOrdersAsync(userId);

        return orders.Select(o => new OrderViewModel
        {
            OrderId = o.Id,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            DeliveryDate = o.DeliveryDate,
            IsDelivered = o.IsDelivered,
            IsDelete = o.IsDelete,
            OrderItems = o.OrderItems.Where(oi => !oi.IsDelete).Select(oi => new OrderItemViewModel
            {
                OrderItemId = oi.Id,
                OrderId = (int)oi.OrderId,
                ItemId = oi.ItemId,
                ItemName = oi.ItemName,
                Price = oi.Price,
                Quantity = oi.Quantity
            }).ToList()
        }).ToList();
    }

    // public async Task<OrderViewModel> GetOrderByIdAsync(int orderId, int userId)
    // {
    //     var order = await _orderRepo.GetOrderByIdAsync(orderId, userId);
    //     if (order == null) return null;

    //     return new OrderViewModel
    //     {
    //         OrderId = order.Id,
    //         OrderDate = order.OrderDate,
    //         TotalAmount = order.TotalAmount,
    //         DeliveryDate = order.DeliveryDate,
    //         IsDelivered = order.IsDelivered,
    //         IsDelete = order.IsDelete,
    //         OrderItems = order.OrderItems.Where(oi => !oi.IsDelete).Select(oi => new OrderItemViewModel
    //         {
    //             OrderItemId = oi.Id,
    //             OrderId = oi.OrderId,
    //             ItemId = oi.ItemId,
    //             ItemName = oi.ItemName,
    //             Price = oi.Price,
    //             Quantity = oi.Quantity
    //         }).ToList()
    //     };
    // }    

    public async Task<PaginationViewModel<OrderViewModel>> GetOrderList(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5, string Status = "")
    {
        IQueryable<Order>? query = _orderRepo.GetOrderList();

        // Apply search 
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower();
            query = query.Where(u =>
                u.Id.ToString().ToLower().Contains(lowerSearchTerm) ||
                u.TotalAmount.ToString().Contains(lowerSearchTerm) ||
            );
        }

        // Apply filter
        if (!string.IsNullOrEmpty(Status) && Status != "All Status")
        {
            query = query;
        }
        else if (Status == "pending")
        {
            query = query.Where(o => !o.IsDelivered);
        }
        else
        {
            query = query.Where(o => o.IsDelivered);
        }

        // Get total records count (before pagination)
        int totalCount = query.Count();

        //sorting
        switch (sortColumn)
        {
            case "ID":
                query = sortDirection == "asc" ? query.OrderBy(u => u.Id) : query.OrderByDescending(u => u.Id);
                break;
            case "Date":
                query = sortDirection == "asc" ? query.OrderBy(u => u.OrderDate) : query.OrderByDescending(u => u.OrderDate);
                break;
            case "Amount":
                query = sortDirection == "asc" ? query.OrderBy(u => u.TotalAmount) : query.OrderByDescending(u => u.TotalAmount);
                break;
        }

        // Apply pagination
        List<OrderViewModel>? items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginationViewModel<OrderViewModel>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<OrderViewModel> GetOrderById(int OrderId)
    {
        var Orders = await _orderRepo.GetOrderList();

        if (Orders == null) return new OrderViewModel();

        var orderData = Orders
        .Where(order => order.Id == OrderId)
        .Select(order => new OrderViewModel
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            CustomerName = order.CreatedByUser.Username,
            TotalAmount = order.TotalAmount,
            DeliveryDate = order.DeliveryDate,
            IsDelivered = order.IsDelivered,
            IsDelete = order.IsDelete,
            OrderItems = order.OrderItems.Where(oi => !oi.IsDelete).Select(oi => new OrderItemViewModel
            {
                OrderItemId = oi.Id,
                OrderId = (int)oi.OrderId,
                ItemId = oi.ItemId,
                ItemName = oi.ItemName,
                ImageURL = oi.Items.ItemImages.Where(item => item.ItemId == oi.ItemId).Select(item => item.ImageURL).FirstOrDefault(),
                Price = oi.Price,
                Quantity = oi.Quantity
            }).ToList()
        }).FirstOrDefault();

        return orderData;
    }

    public async Task<bool> MarkOrderStatus(int orderId)
    {
        return await _orderRepo.MarkOrderStatus(orderId);
    }



}