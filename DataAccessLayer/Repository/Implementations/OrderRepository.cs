// using DataAccessLayer.Context;
// using DataAccessLayer.Models;
// using DataAccessLayer.Repository.Interfaces;
// using DataAccessLayer.ViewModels;
// using Microsoft.EntityFrameworkCore;

// namespace DataAccessLayer.Repository.Implementations;

// public class OrderRepository : IOrderRepository
// {
//     private readonly ApplicationDbContext _db;

//     public OrderRepository(ApplicationDbContext db)
//     {
//         _db = db;
//     }

//     public async Task<int> CreateOrder(int createdBy, List<CartViewModel> cartItems)
//     {
//         var order = new Order
//         {
//             CreatedBy = createdBy,
//             OrderDate = DateTime.Now,
//             TotalAmount = cartItems.Sum(item => item.Price * item.Quantity),
//             Status = "Pending",
//             IsDelivered = false
//         };

//         var orderItems = cartItems.Select(item => new OrderItem
//         {
//             ItemId = item.ItemId,
//             ItemName = item.ItemName,
//             Price = item.Price,
//             Quantity = item.Quantity,
//             ThumbnailImageUrl = item.ThumbnailImageUrl
//         }).ToList();

//         order.OrderItems = orderItems;
//         _db.Orders.Add(order);
//         await _db.SaveChangesAsync();
//         return order.Id;
//     }

//     public async Task<List<OrderViewModel>> GetUserOrders(int createdBy)
//     {
//         return await _db.Orders
//             .Where(o => o.CreatedBy == createdBy)
//             .Include(o => o.OrderItems)
//             .Select(o => new OrderViewModel
//             {
//                 Id = o.Id,
//                 OrderDate = o.OrderDate,
//                 TotalAmount = o.TotalAmount,
//                 Status = o.Status,
//                 IsDelivered = o.IsDelivered,
//                 Items = o.OrderItems.Select(oi => new OrderItemViewModel
//                 {
//                     ItemId = oi.ItemId,
//                     ItemName = oi.ItemName,
//                     Price = oi.Price,
//                     Quantity = oi.Quantity,
//                     ThumbnailImageUrl = oi.ThumbnailImageUrl
//                 }).ToList()
//             })
//             .ToListAsync();
//     }

//     public async Task<bool> UpdateOrder(int orderId, decimal newTotalAmount)
//     {
//         var order = await _db.Orders.FindAsync(orderId);
//         if (order == null) return false;
//         order.TotalAmount = newTotalAmount;
//         await _db.SaveChangesAsync();
//         return true;
//     }

//     public async Task<bool> DeleteOrder(int orderId)
//     {
//         var order = await _db.Orders.FindAsync(orderId);
//         if (order == null) return false;
//         _db.Orders.Remove(order);
//         await _db.SaveChangesAsync();
//         return true;
//     }
// }