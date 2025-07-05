using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface INotificationService
{
    List<NotificationViewModel> GetNotificationsById(int UserId);
    Task<bool> MarkNotificationAsRead(int userNotificationId);
    Task<bool> MarkAllNotificationsAsRead(int userId);
    // NEW
    // Modified: Use NotificationViewModel for stock notification requests
    Task<bool> RequestStockNotificationAsync(int userId, int itemId, string itemName);
    
    Task<bool> HasUserRequestedNotification(int userId, int itemId);
    
    // Modified: Return NotificationViewModel for pending notifications
    Task<List<NotificationViewModel>> GetPendingStockNotificationsAsync(int itemId);

    // Added: Mark original stock notification request as read
    Task<bool> MarkStockNotificationAsReadAsync(int userNotificationId);
    Task<int> CreateNotificationAsync(Notification notification);
    Task<bool> CreateUserNotificationAsync(UserNotification userNotification);
    
}
