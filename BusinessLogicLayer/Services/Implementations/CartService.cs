using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Implementations;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<bool> AddToCart(int userId, int itemId, int quantity = 1)
    {
        return await _cartRepository.AddToCart(userId, itemId, quantity);
    }

    public async Task<List<CartViewModel>> GetCartItems(int userId)
    {
        return await _cartRepository.GetCartItems(userId);
    }

    public async Task<bool> RemoveFromCart(int cartId, int userId)
    {
        return await _cartRepository.RemoveFromCart(cartId, userId);
    }

    // CHANGED: Added implementation for updating cart quantity
    public async Task<bool> UpdateCartQuantity(int cartId, int userId, int quantity)
    {
        return await _cartRepository.UpdateCartQuantity(cartId, userId, quantity);
    }

    // CHANGED: Added implementation for adding all wishlist items to cart
    public async Task<bool> AddAllFromWishlistToCart(int userId)
    {
        return await _cartRepository.AddAllFromWishlistToCart(userId);
    }
}