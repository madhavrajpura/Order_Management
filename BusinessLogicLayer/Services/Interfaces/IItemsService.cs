using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IItemsService
{
    PaginationViewModel<ItemViewModel> GetItemList(int pageNumber = 1, string search = "", int pageSize = 5, string sortColumn = "", string sortDirection = "");
    ItemViewModel GetItemById(int ItemId);
    Task<bool> SaveItem(ItemViewModel itemVM, int UserId,List<string> NewAdditionalImagesURL);
    Task<bool> DeleteItem(int ItemId,int UserId);
    Task<bool> CheckItemExists(ItemViewModel itemVM);
    Task<bool> IsItemInStock(int ItemId);
}
