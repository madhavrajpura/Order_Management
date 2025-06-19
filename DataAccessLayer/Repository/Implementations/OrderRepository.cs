using DataAccessLayer.Context;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _db;

    public OrderRepository(ApplicationDbContext db) => _db = db;

    public IQueryable<OrderViewModel> GetAllOrders()
    {
        try
        {
            return _db.Orders
                .Where(order => !order.IsDelete)
                .Join(_db.Items,
                    order => order.ItemId,
                    item => item.Id,
                    (order, item) => new OrderViewModel
                    {
                        OrderId = order.Id,
                        ItemId = order.ItemId,
                        CustomerName = order.CustomerName,
                        OrderAmount = order.OrderAmount,
                        ItemName = item.Name,
                        Quantity = order.Quantity,
                        OrderDate = order.OrderDate,
                        DeliveryDate = order.DeliveryDate,
                        Price = item.Price
                    })
                .OrderBy(order => order.OrderDate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllOrders: {ex.Message}");
            throw new Exception("Error retrieving orders", ex);
        }
    }

    public OrderViewModel GetOrderById(int orderId)
    {
        try
        {
            return _db.Orders
                .Where(order => order.Id == orderId && !order.IsDelete)
                .Join(_db.Items,
                    order => order.ItemId,
                    item => item.Id,
                    (order, item) => new OrderViewModel
                    {
                        OrderId = order.Id,
                        ItemId = order.ItemId,
                        CustomerName = order.CustomerName,
                        OrderAmount = order.OrderAmount,
                        ItemName = item.Name,
                        Quantity = order.Quantity,
                        OrderDate = order.OrderDate,
                        DeliveryDate = order.DeliveryDate,
                        Price = item.Price
                    })
                .FirstOrDefault() ?? new OrderViewModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetOrderById: {ex.Message}");
            throw new Exception("Error retrieving order by ID", ex);
        }
    }

    public bool SaveOrder(OrderViewModel orderVM, int userId)
    {
        try
        {
            var item = _db.Items.FirstOrDefault(i => i.Id == orderVM.ItemId && !i.IsDelete);
            if (item == null) return false;

            if (orderVM.OrderId == 0)
            {
                Order order = new Order
                {
                    ItemId = orderVM.ItemId,
                    CustomerName = orderVM.CustomerName,
                    OrderAmount = orderVM.Quantity * item.Price,
                    Quantity = orderVM.Quantity,
                    OrderDate = orderVM.OrderDate,
                    DeliveryDate = orderVM.DeliveryDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };
                _db.Orders.Add(order);
            }
            else
            {
                var existingOrder = _db.Orders.FirstOrDefault(o => o.Id == orderVM.OrderId && !o.IsDelete);
                if (existingOrder == null || (DateTime.UtcNow - existingOrder.OrderDate).TotalDays > 2) return false;

                existingOrder.ItemId = orderVM.ItemId;
                existingOrder.CustomerName = orderVM.CustomerName;
                existingOrder.OrderAmount = orderVM.Quantity * item.Price;
                existingOrder.Quantity = orderVM.Quantity;
                existingOrder.OrderDate = orderVM.OrderDate;
                existingOrder.DeliveryDate = orderVM.DeliveryDate;
                existingOrder.UpdatedAt = DateTime.UtcNow;
                existingOrder.UpdatedBy = userId;
                _db.Orders.Update(existingOrder);
            }

            _db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveOrder: {ex.Message}");
            throw new Exception("Error saving order", ex);
        }
    }

    public bool DeleteOrder(int orderId, int userId)
    {
        try
        {
            var order = _db.Orders.FirstOrDefault(o => o.Id == orderId && !o.IsDelete);
            if (order == null) return false;

            order.IsDelete = true;
            order.DeletedAt = DateTime.UtcNow;
            order.DeletedBy = userId;
            _db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteOrder: {ex.Message}");
            throw new Exception("Error deleting order", ex);
        }
    }

    public bool CheckOrderExists(OrderViewModel orderVM)
    {
        try
        {
            return _db.Orders.Any(o => o.Id != orderVM.OrderId && o.CustomerName.ToLower().Trim() == orderVM.CustomerName.ToLower().Trim() && o.ItemId == orderVM.ItemId && !o.IsDelete);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CheckOrderExists: {ex.Message}");
            throw new Exception("Error checking order existence", ex);
        }
    }

    public List<ItemViewModel> GetItemsForAutocomplete(string searchTerm)
    {
        try
        {
            return _db.Items
                .Where(i => !i.IsDelete && (string.IsNullOrEmpty(searchTerm) || i.Name.ToLower().Contains(searchTerm.ToLower())))
                .Select(i => new ItemViewModel
                {
                    ItemId = i.Id,
                    ItemName = i.Name,
                    Price = i.Price
                })
                .Take(10)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetItemsForAutocomplete: {ex.Message}");
            throw new Exception("Error retrieving items for autocomplete", ex);
        }
    }
}