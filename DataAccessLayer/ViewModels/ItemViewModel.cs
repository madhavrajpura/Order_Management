using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.ViewModels;

public class ItemViewModel
{
    public int ItemId { get; set; }

    [Required(ErrorMessage = "Item Name is required")]
    [StringLength(100, ErrorMessage = "Item Name cannot exceed 100 characters.")]
    public string ItemName { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }
    
}