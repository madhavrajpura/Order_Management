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
        try
        {
            if (userId == 0 || itemId == 0) return null;
            var cartItem = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ItemId == itemId);
            return cartItem ?? null; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCartItem: {ex.Message}");
            throw new Exception("Error retrieving CartItem by ID", ex);
        }
    }

    public async Task<bool> AddToCart(int userId, int itemId, int quantity = 1)
    {
        try
        {
            if (userId == 0 || itemId == 0) return false;
            var existingCartItem = await GetCartItem(userId, itemId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                _db.Carts.Update(existingCartItem);
            }
            else
            {
                var cart = new Cart
                {
                    UserId = userId,
                    ItemId = itemId,
                    Quantity = quantity, 
                    CreatedAt = DateTime.Now
                };
                _db.Carts.Add(cart);
            }
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddToCart: {ex.Message}");
            throw new Exception("Error in AddToCart", ex);
        }
    }

    public async Task<List<CartViewModel>> GetCartItems(int userId)
    {
        try
        {
            if (userId == 0) return new List<CartViewModel>();
            return await _db.Carts.Include(c => c.Item).ThenInclude(c => c.ItemImages)
                .Where(c => c.UserId == userId)
                .Select(c => new CartViewModel
                {
                    Id = c.Id,
                    ItemId = c.ItemId,
                    ItemName = c.Item.Name,
                    Price = c.Item.Price,
                    ThumbnailImageUrl = c.Item.ItemImages.Select(i => i.ImageURL).FirstOrDefault() ?? string.Empty,
                    Quantity = c.Quantity 
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCartItems: {ex.Message}");
            throw new Exception("Error retrieving CartItems by ID", ex);
        }
    }

    public async Task<bool> RemoveFromCart(int cartId, int userId)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var cartItem = await _db.Carts.FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId);
            if (cartItem != null)
            {
                _db.Carts.Remove(cartItem);
                await _db.SaveChangesAsync();
            }
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error in RemoveFromCart: {ex.Message}");
            throw new Exception("Error in RemoveFromCart", ex);
        }
    }

    public async Task<bool> UpdateCartQuantity(int cartId, int userId, int quantity)
    {
        try
        {
            var cartItem = await _db.Carts.FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId);
            if (cartItem == null) return false;

            cartItem.Quantity = quantity;
            _db.Carts.Update(cartItem);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateCartQuantity: {ex.Message}");
            throw new Exception("Error updating cart quantity", ex);
        }
    }

    public async Task<bool> AddAllFromWishlistToCart(int userId)
    {
        try
        {
            var wishlistItems = await _db.WishLists
                .Where(w => w.LikedBy == userId)
                .Select(w => w.ItemId)
                .ToListAsync();

            foreach (var itemId in wishlistItems)
            {
                var existingCartItem = await GetCartItem(userId, itemId);
                if (existingCartItem == null)
                {
                    var cart = new Cart
                    {
                        UserId = userId,
                        ItemId = itemId,
                        Quantity = 1, 
                        CreatedAt = DateTime.Now
                    };
                    _db.Carts.Add(cart);
                }
            }
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddAllFromWishlistToCart: {ex.Message}");
            throw new Exception("Error adding wishlist items to cart", ex);
        }
    }
}