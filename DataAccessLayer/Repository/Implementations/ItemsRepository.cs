using DataAccessLayer.Context;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;

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
            return _db.Items
                .Where(item => !item.IsDelete)
                .Select(item => new ItemViewModel
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    Price = item.Price,
                    CreatedAt = item.CreatedAt,
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
            return _db.Items.Where(item => item.Id == ItemId && !item.IsDelete)
                .Select(item => new ItemViewModel
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    Price = item.Price,
                    CreatedAt = item.CreatedAt,
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
        try
        {
            if (itemVM.ItemId == 0)
            {
                Item item = new Item
                {
                    Id = itemVM.ItemId,
                    Name = itemVM.ItemName,
                    Price = itemVM.Price,
                    CreatedAt = DateTime.Now,
                    CreatedBy = UserId
                };
                _db.Items.Add(item);
            }
            else
            {
                Item? ExistingItem = _db.Items.FirstOrDefault(item => item.Id == itemVM.ItemId && !item.IsDelete);
                if (ExistingItem == null) return false;

                ExistingItem.Name = itemVM.ItemName;
                ExistingItem.Price = itemVM.Price;
                ExistingItem.UpdatedAt = DateTime.Now;
                ExistingItem.UpdatedBy = UserId;
                _db.Items.Update(ExistingItem);
            }

            _db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveItem: {ex.Message}");
            throw new Exception("Error saving item", ex);
        }
    }

    public bool DeleteItem(int ItemId, int UserId)
    {
        try
        {
            Item? item = _db.Items.FirstOrDefault(item => item.Id == ItemId && !item.IsDelete);

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