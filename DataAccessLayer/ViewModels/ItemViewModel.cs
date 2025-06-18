using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.ViewModels;

public class ItemViewModel
{
    public int ItemId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Item Name is required")]
    [StringLength(100, ErrorMessage = "Item Name cannot exceed 100 characters.")]
    public string ItemName { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    // [Required(ErrorMessage = "Quantity is required")]
    // [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    // public int Quantity { get; set; }

}