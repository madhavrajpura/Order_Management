namespace DataAccessLayer.ViewModels;

public class UserMainViewModel
{
    public int UserId { get; set; }
    public string CustomerName { get; set; } = null!;
    public List<OrderViewModel> orderListVM { get; set; } = null!;
}
