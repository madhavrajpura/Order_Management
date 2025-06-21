using DataAccessLayer.Context;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class ItemsRepository : IItemsRepository
{
    private readonly ApplicationDbContext _db;
    public ItemsRepository(ApplicationDbContext db) => _db = db;

    #region Items CRUD

    public IQueryable<ItemViewModel> GetAllItem()
    {
        try
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
                    ThumbnailImageUrl = item.ItemImages.FirstOrDefault(itemImage => itemImage.IsThumbnail).ImageURL
                })
                .OrderBy(item => item.CreatedAt) ?? Enumerable.Empty<ItemViewModel>().AsQueryable();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllItem: {ex.Message}");
            throw new Exception("Error retrieving items", ex);
        }
    }

    public ItemViewModel GetItemById(int ItemId)
    {
        try
        {
            return _db.Items.Include(item => item.ItemImages).Where(item => item.Id == ItemId && !item.IsDelete)
                .Select(item => new ItemViewModel
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    Price = item.Price,
                    CreatedAt = item.CreatedAt,
                    Details = item.Details,
                    ThumbnailImageUrl = item.ItemImages.FirstOrDefault(itemImage => itemImage.IsThumbnail).ImageURL,
                })
                .FirstOrDefault() ?? new ItemViewModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetItemById: {ex.Message}");
            throw new Exception("Error retrieving item by ID", ex);
        }
    }

    public bool SaveItem(ItemViewModel itemVM, int UserId)
    {
        if (itemVM == null) return false;

        if (itemVM.ItemId == 0)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                Item newItem = new Item
                {
                    Id = itemVM.ItemId,
                    Name = itemVM.ItemName,
                    Price = itemVM.Price,
                    Details = itemVM.Details,
                    CreatedAt = DateTime.Now,
                    CreatedBy = UserId
                };
                _db.Items.Add(newItem);
                _db.SaveChanges();

                if (itemVM.ThumbnailImageFile != null)
                {
                    ItemImages thumbnailImage = new ItemImages
                    {
                        ItemId = newItem.Id,
                        ImageURL = itemVM.ThumbnailImageUrl,
                        IsThumbnail = true,
                        CreatedAt = DateTime.Now,
                        CreatedBy = UserId
                    };
                    _db.ItemImages.Add(thumbnailImage);
                }

                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
        else
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                Item? ExistingItem = _db.Items.FirstOrDefault(item => item.Id == itemVM.ItemId && !item.IsDelete);
                if (ExistingItem == null) return false;

                ExistingItem.Name = itemVM.ItemName;
                ExistingItem.Price = itemVM.Price;
                ExistingItem.Details = itemVM.Details;
                ExistingItem.UpdatedAt = DateTime.Now;
                ExistingItem.UpdatedBy = UserId;
                _db.Items.Update(ExistingItem);
                _db.SaveChanges();

                // Handle thumbnail image
                if (itemVM.ThumbnailImageFile != null)
                {
                    ItemImages? existingThumbnail = _db.ItemImages
                        .FirstOrDefault(pi => pi.ItemId == ExistingItem.Id && pi.IsThumbnail);

                    if (existingThumbnail != null)
                    {
                        existingThumbnail.ImageURL = itemVM.ThumbnailImageUrl;
                        existingThumbnail.UpdatedAt = DateTime.Now;
                        existingThumbnail.UpdatedBy = UserId;
                        _db.ItemImages.Update(existingThumbnail);
                    }
                }

                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }

    public bool DeleteItem(int ItemId, int UserId)
    {
        try
        {
            Item? item = _db.Items.FirstOrDefault(item => item.Id == ItemId && !item.IsDelete);

            // Notification notification = _db.Notifications.Where(item => item.)   

            if (item != null)
            {
                item.IsDelete = true;
                item.DeletedAt = DateTime.Now;
                item.DeletedBy = UserId;
                _db.SaveChanges();
                return true;
            }
            return false;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteItem: {ex.Message}");
            throw new Exception("Error deleting item", ex);
        }
    }

    public bool CheckItemExists(ItemViewModel itemVM)
    {
        try
        {
            if (itemVM.ItemId == 0) return _db.Items.Any(x => x.Name.ToLower().Trim() == itemVM.ItemName.ToLower().Trim() && !x.IsDelete);
            return _db.Items.Any(x => x.Name.ToLower().Trim() == itemVM.ItemName.ToLower().Trim() && !x.IsDelete && x.Id != itemVM.ItemId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CheckItemExists: {ex.Message}");
            throw new Exception("Error checking item existence", ex);
        }
    }

    #endregion

}