using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.ViewModels;

public class CartViewModel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = null!;
    public decimal Price { get; set; }
    public string ThumbnailImageUrl { get; set; } = null!;
    public int Quantity { get; set; } 
}