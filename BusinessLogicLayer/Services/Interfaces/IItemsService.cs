using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IItemsService
{
    PaginationViewModel<ItemViewModel> GetItemList(int pageNumber = 1, string search = "", int pageSize = 5, string sortColumn = "", string sortDirection = "");
    ItemViewModel GetItemById(int ItemId);
    bool SaveItem(ItemViewModel itemVM, int UserId);
    bool DeleteItem(int ItemId,int UserId);
    bool CheckItemExists(ItemViewModel itemVM);
}
