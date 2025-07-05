using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class CartRepository : ICartRepository
{
    private readonly NewItemOrderDbContext _db;

    public CartRepository(NewItemOrderDbContext db)
    {
        _db = db;
    }

    public async Task<Cart> GetCartItem(int userId, int itemId)
    {
        Cart? cartItem = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ItemId == itemId) ?? null;
        return cartItem!;
    }

    public async Task<bool> IsItemExistsInCart(int userId, int itemId)
    {
        return await _db.Carts.AnyAsync(c => c.UserId == userId && c.ItemId == itemId);
    }

    public async Task<bool> AddToCart(int userId, int itemId, int quantity = 1)
    {
        Cart? cart = new Cart
        {
            UserId = userId,
            ItemId = itemId,
            Quantity = quantity,
            CreatedAt = DateTime.Now
        };
        _db.Carts.Add(cart);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<CartViewModel>> GetCartItems(int userId)
    {
        List<CartViewModel>? cartItem = await _db.Carts.Include(c => c.Item).ThenInclude(c => c.ItemImages)
            .Where(c => c.UserId == userId)
            .Select(c => new CartViewModel
            {
                Id = c.Id,
                ItemId = c.ItemId,
                ItemName = c.Item.Name,
                Price = c.Item.Price,
                ThumbnailImageUrl = c.Item.ItemImages.Select(i => i.ImageUrl).FirstOrDefault() ?? string.Empty,
                Quantity = c.Quantity,
                Stock = c.Item.Stock
            })
            .ToListAsync() ?? throw new Exception();
        return cartItem;
    }

    public async Task<bool> RemoveFromCart(int cartId, int userId)
    {
        Cart? cartItem = await _db.Carts.FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId) ?? throw new Exception();
        _db.Carts.Remove(cartItem);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateCartQuantity(int cartId, int userId, int quantity)
    {

        Cart? cartItem = await _db.Carts.FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId) ?? throw new Exception();
        cartItem.Quantity = quantity;
        _db.Carts.Update(cartItem);
        await _db.SaveChangesAsync();
        return true;
    }


    public async Task<bool> AddAllFromWishlistToCart(int userId)
    {
        List<int>? wishlistItems = await _db.WishLists
        .Where(w => w.LikedBy == userId)
        .Select(w => w.ItemId)
        .ToListAsync() ?? throw new Exception();

        foreach (int itemId in wishlistItems)
        {
            Cart? existingCartItem = await GetCartItem(userId, itemId);

            if (existingCartItem == null)
            {
                Cart? cart = new Cart
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

    public async Task<CartViewModel> GetCartItemById(int CartId, int UserId)
    {
        CartViewModel? cartItem = await _db.Carts.Where(c => c.UserId == UserId && c.Id == CartId).Select(c => new CartViewModel
        {
            ItemId = c.ItemId,
            ItemName = c.Item.Name,
            Price = c.Item.Price,
            Stock = c.Item.Stock,
            Quantity = c.Quantity
        }).FirstOrDefaultAsync() ?? null;

        return cartItem!;
    }

}