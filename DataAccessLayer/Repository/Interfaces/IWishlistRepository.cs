using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface IWishlistRepository
{
    Task<bool> ToggleWishlistItem(int userId, int itemId);
    Task<List<WishListViewModel>> GetUserWishlist(int userId);
    Task<bool> IsItemInWishlist(int userId, int itemId);
}
