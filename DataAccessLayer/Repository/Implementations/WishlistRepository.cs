using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class WishlistRepository : IWishlistRepository
{
    private readonly NewItemOrderDbContext _db;

    public WishlistRepository(NewItemOrderDbContext db)
    {
        _db = db;
    }

    public async Task<bool> ToggleWishlistItem(int userId, int itemId)
    {
         WishList? existing = await _db.WishLists
            .FirstOrDefaultAsync(w => w.LikedBy == userId && w.ItemId == itemId);

        if (existing != null)
        {
            _db.WishLists.Remove(existing);
            await _db.SaveChangesAsync();
            return false;
        }

        _db.WishLists.Add(new WishList
        {
            ItemId = itemId,
            LikedBy = userId,
            LikedAt = DateTime.Now
        });

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<WishListViewModel>> GetUserWishlist(int userId)
    {
        return await _db.WishLists
            .Include(w => w.Item)
            .ThenInclude(wi => wi.ItemImages)
            .Where(w => w.LikedBy == userId)
            .Select(w => new WishListViewModel
            {
                WhishListId = w.Id,
                ItemId = w.Item.Id,
                ItemName = w.Item.Name,
                Price = w.Item.Price,
                Details = w.Item.Details,
                Stock = w.Item.Stock,
                ThumbnailImageUrl = w.Item.ItemImages.FirstOrDefault(itemImage => itemImage.IsThumbnail).ImageUrl,
                LikedAt = (DateTime)w.LikedAt
            })
            .ToListAsync();
    }

    public async Task<bool> IsItemInWishlist(int userId, int itemId)
    {
        return await _db.WishLists
            .AnyAsync(w => w.LikedBy == userId && w.ItemId == itemId);
    }

}