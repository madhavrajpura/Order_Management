using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IDashboardService
{
    Task<List<TopItemViewModel>> GetTopSoldItems(int topN = 5);
    Task<List<TopUserViewModel>> GetTopActiveUsers(int topN = 5);
    Task<List<TopItemViewModel>> GetTopLikedItems(int topN = 5);
    Task<Dictionary<string, OrderStatsViewModel>> GetOrderStats();
}
