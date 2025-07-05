using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class ItemsRepository : IItemsRepository
{
    private readonly NewItemOrderDbContext _db;
    public ItemsRepository(NewItemOrderDbContext db) => _db = db;

    #region Items CRUD

    public IQueryable<ItemViewModel> GetAllItem()
    {
        return _db.Items.Include(item => item.ItemImages)
            .Where(item => !item.IsDelete)
            .Select(item => new ItemViewModel
            {
                ItemId = item.Id,
                ItemName = item.Name,
                Price = item.Price,
                CreatedAt = item.CreatedAt,
                Details = item.Details,
                Stock = item.Stock,
                ThumbnailImageUrl = item.ItemImages.FirstOrDefault(itemImage => itemImage.IsThumbnail).ImageUrl,
                AdditionalImagesUrl = item.ItemImages.Where(itemImage => !itemImage.IsThumbnail).Select(image => image.ImageUrl).ToList()
            })
            .OrderBy(item => item.CreatedAt) ?? throw new Exception();
    }

    public ItemViewModel GetItemById(int ItemId)
    {
        ItemViewModel? item = _db.Items.Include(item => item.ItemImages)
            .Where(item => item.Id == ItemId && !item.IsDelete)
            .Select(item => new ItemViewModel
            {
                ItemId = item.Id,
                ItemName = item.Name,
                Price = item.Price,
                CreatedAt = item.CreatedAt,
                Details = item.Details,
                Stock = item.Stock,
                ThumbnailImageUrl = item.ItemImages.FirstOrDefault(itemImage => itemImage.IsThumbnail).ImageUrl,
                AdditionalImagesUrl = item.ItemImages.Where(itemImage => !itemImage.IsThumbnail).Select(image => image.ImageUrl).ToList()
            })
            .FirstOrDefault() ?? throw new Exception();
        return item;
    }

    public async Task<bool> SaveItem(ItemViewModel itemVM, int UserId, List<string> NewAdditionalImagesURL)
    {
        if (itemVM.ItemId == 0)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                Item newItem = new Item
                {
                    Id = itemVM.ItemId,
                    Name = itemVM.ItemName,
                    Price = itemVM.Price,
                    Details = itemVM.Details,
                    Stock = itemVM.Stock,
                    CreatedAt = DateTime.Now,
                    CreatedBy = UserId
                };
                await _db.Items.AddAsync(newItem);
                await _db.SaveChangesAsync();

                if (itemVM.ThumbnailImageFile != null)
                {
                    ItemImage thumbnailImage = new ItemImage
                    {
                        ItemId = newItem.Id,
                        ImageUrl = itemVM.ThumbnailImageUrl!,
                        IsThumbnail = true,
                        CreatedAt = DateTime.Now,
                        CreatedBy = UserId
                    };
                    await _db.ItemImages.AddAsync(thumbnailImage);
                }

                if (itemVM.AdditionalImagesFile?.Count > 0)
                {
                    foreach (string? AdditionalImages in NewAdditionalImagesURL!)
                    {
                        ItemImage thumbnailImage = new ItemImage
                        {
                            ItemId = newItem.Id,
                            ImageUrl = AdditionalImages,
                            IsThumbnail = false,
                            CreatedAt = DateTime.Now,
                            CreatedBy = UserId
                        };
                        await _db.ItemImages.AddAsync(thumbnailImage);
                    }
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception();
            }
        }
        else
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                Item? ExistingItem = await _db.Items.FirstOrDefaultAsync(item => item.Id == itemVM.ItemId && !item.IsDelete)
                    ?? throw new Exception();

                ExistingItem.Name = itemVM.ItemName;
                ExistingItem.Price = itemVM.Price;
                ExistingItem.Details = itemVM.Details;
                ExistingItem.Stock = itemVM.Stock;
                ExistingItem.UpdatedAt = DateTime.Now;
                ExistingItem.UpdatedBy = UserId;
                _db.Items.Update(ExistingItem);
                await _db.SaveChangesAsync();

                // Handle thumbnail image
                if (itemVM.ThumbnailImageFile != null)
                {
                    ItemImage? existingThumbnail = await _db.ItemImages
                        .FirstOrDefaultAsync(pi => pi.ItemId == ExistingItem.Id && pi.IsThumbnail);

                    if (existingThumbnail != null)
                    {
                        existingThumbnail.ImageUrl = itemVM.ThumbnailImageUrl;
                        existingThumbnail.UpdatedAt = DateTime.Now;
                        existingThumbnail.UpdatedBy = UserId;
                        _db.ItemImages.Update(existingThumbnail);
                    }
                }

                List<ItemImage> ExistingAdditionalImagesDB = await _db.ItemImages
                    .Where(i => i.ItemId == ExistingItem.Id && !i.IsThumbnail).ToListAsync();

                if (ExistingAdditionalImagesDB != null && itemVM.AdditionalImagesUrl != null)
                {
                    foreach (ItemImage? existingImageDB in ExistingAdditionalImagesDB)
                    {
                        if (!itemVM.AdditionalImagesUrl.Contains(existingImageDB.ImageUrl))
                        {
                            _db.ItemImages.Remove(existingImageDB);
                        }
                    }
                }

                foreach (string? NewAdditionalImage in NewAdditionalImagesURL)
                {
                    ItemImage newImages = new ItemImage
                    {
                        ItemId = itemVM.ItemId,
                        ImageUrl = NewAdditionalImage,
                        IsThumbnail = false,
                        CreatedAt = DateTime.Now,
                        CreatedBy = UserId
                    };
                    await _db.ItemImages.AddAsync(newImages);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception();
            }
        }
    }

    public async Task<bool> DeleteItem(int ItemId, int UserId)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            Item? item = await _db.Items.FirstOrDefaultAsync(item => item.Id == ItemId && !item.IsDelete)
                ?? throw new Exception();

            item.IsDelete = true;
            item.DeletedAt = DateTime.Now;
            item.DeletedBy = UserId;
            _db.Items.Update(item);
            await _db.SaveChangesAsync();

            List<ItemImage>? itemImages = await _db.ItemImages.Where(i => i.ItemId == ItemId && !item.IsDelete).ToListAsync();

            if (itemImages != null)
            {
                foreach (ItemImage? Images in itemImages)
                {
                    _db.ItemImages.Remove(Images);
                }
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw new Exception();
        }
    }

    public async Task<bool> CheckItemExists(ItemViewModel itemVM)
    {
        if (string.IsNullOrEmpty(itemVM.ItemName)) throw new Exception();

        if (itemVM.ItemId == 0)
            return await _db.Items.AnyAsync(x => x.Name.ToLower().Trim() == itemVM.ItemName.ToLower().Trim() && !x.IsDelete);
        return await _db.Items.AnyAsync(x => x.Name.ToLower().Trim() == itemVM.ItemName.ToLower().Trim() && !x.IsDelete && x.Id != itemVM.ItemId);
    }

    public async Task<bool> IsItemInStock(int ItemId)
    {
        return await _db.Items.AnyAsync(item => item.Id == ItemId && !item.IsDelete && item.Stock != 0);
    }

    #endregion
}