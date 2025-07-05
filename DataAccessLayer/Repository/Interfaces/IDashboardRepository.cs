using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface IDashboardRepository
{
    Task<List<TopItemViewModel>> GetTopSoldItems(int topN);
    Task<List<TopUserViewModel>> GetTopActiveUsers(int topN);
    Task<List<TopItemViewModel>> GetTopLikedItems(int topN);
    Task<Dictionary<string, OrderStatsViewModel>> GetOrderStats();
}
