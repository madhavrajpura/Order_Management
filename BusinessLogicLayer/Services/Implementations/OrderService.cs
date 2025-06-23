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
                CreatedBy = userId,
                OrderItems = orderViewModel.OrderItems.Select(oi => new OrderItem
                {
                    ItemId = oi.ItemId,
                    ItemName = oi.ItemName,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                }).ToList()
            };

            var result = await _orderRepo.CreateOrderAsync(order);

            // if (result)
            // {
            //     // Clear cart after successfully Placed order
            //     var cartItems = await _cartRepo.GetCartItems(userId);
            //     foreach (var cartItem in cartItems)
            //     {
            //         await _cartRepo.RemoveFromCart(cartItem.Id, userId);
            //     }
            // }

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
}