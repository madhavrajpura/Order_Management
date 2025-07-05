using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<List<TopItemViewModel>> GetTopSoldItems(int topN = 5)
    {
        if (topN <= 0) throw new CustomException("Invalid number of items requested.");
        return await _dashboardRepository.GetTopSoldItems(topN);
    }

    public async Task<List<TopUserViewModel>> GetTopActiveUsers(int topN = 5)
    {
        if (topN <= 0) throw new CustomException("Invalid number of users requested.");
        return await _dashboardRepository.GetTopActiveUsers(topN);
    }

    public async Task<List<TopItemViewModel>> GetTopLikedItems(int topN = 5)
    {
        if (topN <= 0) throw new CustomException("Invalid number of items requested.");
        return await _dashboardRepository.GetTopLikedItems(topN);
    }

    public async Task<Dictionary<string, OrderStatsViewModel>> GetOrderStats()
    {
        return await _dashboardRepository.GetOrderStats();
    }
}
