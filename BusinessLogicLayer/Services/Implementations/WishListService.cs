using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Implementations;

public class WishListService : IWishListService
{
    private readonly IWishlistRepository _wishlistRepo;

    public WishListService(IWishlistRepository wishlistRepo) => _wishlistRepo = wishlistRepo;
    public async Task<bool> ToggleWishlistItem(int userId, int itemId)
    {
        if (userId <= 0 || itemId <= 0) throw new CustomException("Invalid User ID or Item ID.");
        return await _wishlistRepo.ToggleWishlistItem(userId, itemId);
    }

    public async Task<List<WishListViewModel>> GetUserWishlist(int userId)
    {
        if (userId <= 0) throw new CustomException("Invalid User ID.");
        return await _wishlistRepo.GetUserWishlist(userId);
    }

    public async Task<List<CombinedItemViewModel>> GetItemsWithWishlistStatus(int userId, List<ItemViewModel> items)
    {
        if (userId <= 0) throw new CustomException("Invalid User ID.");

        List<CombinedItemViewModel>? combinedItems = new List<CombinedItemViewModel>();

        foreach (ItemViewModel? item in items)
        {
            combinedItems.Add(new CombinedItemViewModel
            {
                ItemsVM = item,
                IsInWishlist = await _wishlistRepo.IsItemInWishlist(userId, item.ItemId)
            });
        }
        return combinedItems;
    }

    public async Task<bool> IsItemInWishlist(int userId, int itemId)
    {
        if (userId <= 0 || itemId <= 0) throw new CustomException("Invalid User ID or item ID.");
        return await _wishlistRepo.IsItemInWishlist(userId, itemId);
    }

}