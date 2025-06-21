// using DataAccessLayer.Repository.Interfaces;
// using DataAccessLayer.ViewModels;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace BusinessLogicLayer.Services.Implementations;

// public class OrderService : IOrderService
// {
//     private readonly IOrderRepository _orderRepository;

//     public OrderService(IOrderRepository orderRepository)
//     {
//         _orderRepository = orderRepository;
//     }

//     public async Task<int> PlaceOrder(int createdBy, List<CartViewModel> cartItems)
//     {
//         return await _orderRepository.CreateOrder(createdBy, cartItems);
//     }

//     public async Task<List<OrderViewModel>> GetUserOrders(int createdBy)
//     {
//         return await _orderRepository.GetUserOrders(createdBy);
//     }

//     public async Task<bool> UpdateOrder(int orderId, decimal newTotalAmount)
//     {
//         return await _orderRepository.UpdateOrder(orderId, newTotalAmount);
//     }

//     public async Task<bool> DeleteOrder(int orderId)
//     {
//         return await _orderRepository.DeleteOrder(orderId);
//     }
// }