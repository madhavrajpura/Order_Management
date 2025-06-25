using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Implementations;

public class ItemsService : IItemsService
{
    private readonly IItemsRepository _itemRepository;

    public ItemsService(IItemsRepository itemRepository) => _itemRepository = itemRepository;

    public PaginationViewModel<ItemViewModel> GetItemList(int pageNumber, string search, int pageSize, string sortColumn, string sortDirection)
    {
        var query = _itemRepository.GetAllItem();

        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower().Trim();
            query = query.Where(item => item.ItemName.ToLower().Trim().Contains(lowerSearchTerm) || item.Price.ToString().Trim().Contains(lowerSearchTerm));
        }

        int totalCount = query.Count();

        switch (sortColumn)
        {
            case "ID":
                query = sortDirection == "asc" ? query.OrderBy(item => item.ItemId) : query.OrderByDescending(item => item.ItemId);
                break;
            case "Name":
                query = sortDirection == "asc" ? query.OrderBy(item => item.ItemName) : query.OrderByDescending(item => item.ItemName);
                break;
            case "Price":
                query = sortDirection == "asc" ? query.OrderBy(item => item.Price) : query.OrderByDescending(item => item.Price);
                break;
        }

        List<ItemViewModel>? items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginationViewModel<ItemViewModel>(items, totalCount, pageNumber, pageSize);
    }

    public ItemViewModel GetItemById(int ItemId) => _itemRepository.GetItemById(ItemId);

    public async Task<bool> SaveItem(ItemViewModel itemVM,int UserId) => await _itemRepository.SaveItem(itemVM,UserId);

    public async Task<bool> DeleteItem(int ItemId,int UserId) => await _itemRepository.DeleteItem(ItemId,UserId);

    public async Task<bool> CheckItemExists(ItemViewModel itemVM) => await _itemRepository.CheckItemExists(itemVM);

}
