using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface IItemsRepository
{
    IQueryable<ItemViewModel> GetAllItem();
    ItemViewModel GetItemById(int ItemId);
    bool SaveItem(ItemViewModel itemVM, int UserId);
    bool DeleteItem(int ItemId, int UserId);
    bool CheckItemExists(ItemViewModel itemVM);

}