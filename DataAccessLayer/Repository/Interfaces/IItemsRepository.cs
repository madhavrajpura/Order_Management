using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface IItemsRepository
{
    IQueryable<ItemViewModel> GetAllItem();
    ItemViewModel GetItemById(int ItemId);
    Task<bool> SaveItem(ItemViewModel itemVM, int UserId);
    Task<bool> DeleteItem(int ItemId, int UserId);
    Task<bool> CheckItemExists(ItemViewModel itemVM);
}