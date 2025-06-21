using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Implementations;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    public CartService(ICartRepository cartRepo)
    {
        _cartRepo = cartRepo;
    }

    public async Task<bool> AddToCart(int userId, int itemId)
    {
        var existingCartItem = await _cartRepo.GetCartItem(userId, itemId);
        if (existingCartItem != null) return false;

        await _cartRepo.AddToCart(userId, itemId);
        return true;
    }

    public async Task<List<CartViewModel>> GetCartItems(int userId)
    {
        return await _cartRepo.GetCartItems(userId);
    }

    public async Task<bool> RemoveFromCart(int cartId, int userId)
    {
        return await _cartRepo.RemoveFromCart(cartId, userId);
    }
}
