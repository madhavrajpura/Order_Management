using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.ViewModels;

public class OrderViewModel
{
    public int OrderId { get; set; }

    public int ItemId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Customer Name is required")]
    [StringLength(100, ErrorMessage = "Customer Name cannot exceed 100 characters.")]
    public string CustomerName { get; set; } = null!;

    public decimal OrderAmount { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Item Name is required")]
    [StringLength(50, ErrorMessage = "Item Name cannot exceed 50 characters.")]
    public string ItemName { get; set; } = null!;

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Order Date is required")]
    public DateTime OrderDate { get; set; }

    [Required(ErrorMessage = "Delivery Date is required")]
    public DateTime DeliveryDate { get; set; }

    public decimal Price { get; set; }
}
