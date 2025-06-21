using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Implementations;

public class WishListService : IWishListService
{
    private readonly IWishlistRepository _wishlistRepo;

    public WishListService(IWishlistRepository wishlistRepo)
    {
        _wishlistRepo = wishlistRepo;
    }

    public async Task<bool> ToggleWishlistItem(int userId, int itemId)
    {
        return await _wishlistRepo.ToggleWishlistItem(userId, itemId);
    }

    public async Task<List<WishListViewModel>> GetUserWishlist(int userId)
    {
        return await _wishlistRepo.GetUserWishlist(userId);
    }

    public async Task<List<CombinedItemViewModel>> GetItemsWithWishlistStatus(int userId, List<ItemViewModel> items)
    {
        var combinedItems = new List<CombinedItemViewModel>();
        
        foreach (var item in items)
        {
            combinedItems.Add(new CombinedItemViewModel
            {
                ItemsVM = item,
                IsInWishlist = await _wishlistRepo.IsItemInWishlist(userId, item.ItemId)
            });
        }
        return combinedItems;
    }

    public async Task<bool> IsItemInWishlist(int userId, int itemId){
        return await _wishlistRepo.IsItemInWishlist(userId,itemId);
    }

}