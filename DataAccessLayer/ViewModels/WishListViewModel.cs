namespace DataAccessLayer.ViewModels;

public class WishListViewModel
{
    public int WhishListId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public decimal Price { get; set; }
    public string ThumbnailImageUrl { get; set; }
    public DateTime LikedAt { get; set; }

}