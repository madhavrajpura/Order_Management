using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models;
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
        if (userId <= 0 || itemId <= 0) throw new CustomException("Invalid User ID or Item ID.");

        if (quantity <= 0) throw new CustomException("Invalid Quantity");

        return await _cartRepository.AddToCart(userId, itemId, quantity);
    }

    public async Task<bool> IsItemExistsInCart(int userId, int itemId)
    {
        if (userId <= 0 || itemId <= 0) throw new CustomException("Invalid User ID or Item ID.");
        return await _cartRepository.IsItemExistsInCart(userId, itemId);
    }

    public async Task<Cart> GetCartItem(int userId, int itemId)
    {
        if (userId <= 0 || itemId <= 0) throw new CustomException("Invalid User ID or Item ID.");
        return await _cartRepository.GetCartItem(userId, itemId);
    }

    public async Task<List<CartViewModel>> GetCartItems(int userId)
    {
        if (userId <= 0) throw new CustomException("Invalid User ID.");

        return await _cartRepository.GetCartItems(userId);
    }

    public async Task<bool> RemoveFromCart(int cartId, int userId)
    {
        if (userId <= 0 || cartId <= 0) throw new CustomException("Invalid User ID or Cart ID.");

        return await _cartRepository.RemoveFromCart(cartId, userId);
    }

    public async Task<bool> UpdateCartQuantity(int cartId, int userId, int quantity)
    {
        if (userId <= 0 || cartId <= 0) throw new CustomException("Invalid User ID or Cart ID.");

        if (quantity <= 0) throw new CustomException("Invalid Quantity");

        return await _cartRepository.UpdateCartQuantity(cartId, userId, quantity);
    }

    public async Task<bool> AddAllFromWishlistToCart(int userId)
    {
        if (userId <= 0) throw new CustomException("Invalid User ID.");

        return await _cartRepository.AddAllFromWishlistToCart(userId);
    }

    public async Task<CartViewModel> GetCartItemById(int CartId, int UserId)
    {
        if (CartId <= 0 || UserId <= 0) throw new CustomException("Invalid Cart ID or User ID.");
        return await _cartRepository.GetCartItemById(CartId, UserId);
    }

}