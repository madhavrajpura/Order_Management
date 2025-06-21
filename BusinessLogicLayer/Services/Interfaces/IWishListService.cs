using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IWishListService
{
    Task<bool> ToggleWishlistItem(int userId, int itemId);
    Task<List<WishListViewModel>> GetUserWishlist(int userId);
    Task<List<CombinedItemViewModel>> GetItemsWithWishlistStatus(int userId, List<ItemViewModel> items);
        Task<bool> IsItemInWishlist(int userId, int itemId);

}
