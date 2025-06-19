namespace DataAccessLayer.ViewModels;

public class NotificationViewModel
{
    public int NotificationId { get; set; }
    public string? Message { get; set; }
    public int CreatedBy { get; set; }
    public bool IsActive { get; set; }
    public int UserNotificationId { get; set; }
    public int UserId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}
