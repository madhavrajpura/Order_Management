namespace DataAccessLayer.ViewModels;

public class OrderStatsViewModel
{
    public string Period { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}
