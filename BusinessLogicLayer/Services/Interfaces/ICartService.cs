using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface ICartService
{
    Task<bool> AddToCart(int userId, int itemId, int quantity = 1);
    Task<bool> IsItemExistsInCart(int userId, int itemId); // NEW
    Task<Cart> GetCartItem(int userId, int itemId);
    Task<List<CartViewModel>> GetCartItems(int userId);
    Task<bool> RemoveFromCart(int cartId, int userId);
    Task<bool> UpdateCartQuantity(int cartId, int userId, int quantity);
    Task<bool> AddAllFromWishlistToCart(int userId);
    Task<CartViewModel> GetCartItemById(int CartId, int UserId); // NEW
}