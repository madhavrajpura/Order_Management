using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface ICartRepository
{
    Task<Cart> GetCartItem(int userId, int itemId);
    Task<bool> AddToCart(int userId, int itemId);
    Task<List<CartViewModel>> GetCartItems(int userId);
    Task<bool> RemoveFromCart(int cartId, int userId);
}