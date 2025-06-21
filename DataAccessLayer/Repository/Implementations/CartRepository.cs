using DataAccessLayer.Context;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _db;

    public CartRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Cart> GetCartItem(int userId, int itemId)
    {
        return await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ItemId == itemId);
    }

    public async Task<bool> AddToCart(int userId, int itemId)
    {
        var cart = new Cart
        {
            UserId = userId,
            ItemId = itemId,
            CreatedAt = DateTime.Now
        };
        _db.Carts.Add(cart);
        await _db.SaveChangesAsync();
        return true;

    }

    public async Task<List<CartViewModel>> GetCartItems(int userId)
    {
        return await _db.Carts.Include(c => c.Item).ThenInclude(c => c.ItemImages)
            .Where(c => c.UserId == userId)
            .Select(c => new CartViewModel
            {
                Id = c.Id,
                ItemId = c.ItemId,
                ItemName = c.Item.Name,
                Price = c.Item.Price,
                ThumbnailImageUrl = c.Item.ItemImages.Select(i => i.ImageURL).FirstOrDefault() ?? string.Empty,
                Quantity = 1 // Default quantity, managed client-side
            })
            .ToListAsync();
    }

    public async Task<bool> RemoveFromCart(int cartId, int userId)
    {
        var cartItem = await _db.Carts.FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId);
        if (cartItem != null)
        {
            _db.Carts.Remove(cartItem);
            await _db.SaveChangesAsync();
            return true;
        }
        return false;

    }
}
