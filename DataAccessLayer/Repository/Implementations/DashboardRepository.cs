using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace DataAccessLayer.Repository.Implementations;

public class DashboardRepository : IDashboardRepository
{
    private readonly NewItemOrderDbContext _db;

    public DashboardRepository(NewItemOrderDbContext db)
    {
        _db = db;
    }

    public async Task<List<TopItemViewModel>> GetTopSoldItems(int topN)
    {
        var topItems = await _db.OrderItems
            .Include(oi => oi.Item)
            .Where(oi => !oi.Item.IsDelete)
            .GroupBy(oi => new { oi.ItemId, oi.Item.Name })
            .Select(g => new TopItemViewModel
            {
                ItemId = g.Key.ItemId,
                ItemName = g.Key.Name,
                Count = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.Count)
            .Take(topN)
            .ToListAsync();
        
        return topItems.Any() ? topItems : new List<TopItemViewModel>();
    }

    public async Task<List<TopUserViewModel>> GetTopActiveUsers(int topN)
    {
        var topUsers = await _db.Orders
            .Include(o => o.CreatedByNavigation)
            .Where(o => !o.IsDelete)
            .GroupBy(o => new { o.CreatedBy, o.CreatedByNavigation.Username })
            .Select(g => new TopUserViewModel
            {
                UserId = g.Key.CreatedBy,
                Username = g.Key.Username,
                OrderCount = g.Count()
            })
            .OrderByDescending(x => x.OrderCount)
            .Take(topN)
            .ToListAsync();
        
        return topUsers.Any() ? topUsers : new List<TopUserViewModel>();
    }

    public async Task<List<TopItemViewModel>> GetTopLikedItems(int topN)
    {
        var topItems = await _db.WishLists
            .Include(w => w.Item)
            .Where(w => !w.Item.IsDelete)
            .GroupBy(w => new { w.ItemId, w.Item.Name })
            .Select(g => new TopItemViewModel
            {
                ItemId = g.Key.ItemId,
                ItemName = g.Key.Name,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(topN)
            .ToListAsync();
        
        return topItems.Any() ? topItems : new List<TopItemViewModel>();
    }

    public async Task<Dictionary<string, OrderStatsViewModel>> GetOrderStats()
    {
        var now = DateTime.Now;
        var stats = new Dictionary<string, OrderStatsViewModel>
        {
            { "Day", new OrderStatsViewModel { Period = "Day" } },
            { "Week", new OrderStatsViewModel { Period = "Week" } },
            { "Month", new OrderStatsViewModel { Period = "Month" } },
            { "Year", new OrderStatsViewModel { Period = "Year" } }
        };

        var orders = await _db.Orders
            .Where(o => !o.IsDelete)
            .Select(o => new { o.OrderDate, o.TotalAmount })
            .ToListAsync();

        foreach (var order in orders)
        {
            if (order.OrderDate.Date == now.Date)
            {
                stats["Day"].OrderCount++;
                stats["Day"].TotalAmount += order.TotalAmount;
            }
            if (order.OrderDate >= now.AddDays(-7))
            {
                stats["Week"].OrderCount++;
                stats["Week"].TotalAmount += order.TotalAmount;
            }
            if (order.OrderDate >= now.AddMonths(-1))
            {
                stats["Month"].OrderCount++;
                stats["Month"].TotalAmount += order.TotalAmount;
            }
            if (order.OrderDate >= now.AddYears(-1))
            {
                stats["Year"].OrderCount++;
                stats["Year"].TotalAmount += order.TotalAmount;
            }
        }

        return stats;
    }
}