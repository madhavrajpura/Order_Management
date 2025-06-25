namespace DataAccessLayer.ViewModels;

public class OrderViewModel
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public long PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public bool IsDelivered { get; set; }
    public bool IsDelete { get; set; }
    public int CreatedByUser { get; set; }
    public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

}