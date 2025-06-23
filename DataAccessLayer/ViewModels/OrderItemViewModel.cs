namespace DataAccessLayer.ViewModels;

public class OrderItemViewModel
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; } 
    public int ItemId { get; set; } 
    public string ItemName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity{get; set;}

}