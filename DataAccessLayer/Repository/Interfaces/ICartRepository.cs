using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface ICartRepository
{
    Task<Cart> GetCartItem(int userId, int itemId);
    Task<bool> AddToCart(int userId, int itemId, int quantity = 1);
    Task<List<CartViewModel>> GetCartItems(int userId);
    Task<bool> RemoveFromCart(int cartId, int userId);
    // CHANGED: Added method to update cart quantity
    Task<bool> UpdateCartQuantity(int cartId, int userId, int quantity);
    // CHANGED: Added method to add all wishlist items to cart
    Task<bool> AddAllFromWishlistToCart(int userId);
}