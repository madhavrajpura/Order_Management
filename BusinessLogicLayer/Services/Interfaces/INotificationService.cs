using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface INotificationService
{
    List<NotificationViewModel> GetNotificationsById(int UserId);
    Task<bool> MarkNotificationAsRead(int userNotificationId);
    Task<bool> MarkAllNotificationsAsRead(int userId);
    Task<bool> RequestStockNotificationAsync(int userId, int itemId, string itemName);
    Task<bool> HasUserRequestedNotification(int userId, int itemId);
    Task<List<NotificationViewModel>> GetPendingStockNotificationsAsync(int itemId);
    Task<bool> MarkStockNotificationAsReadAsync(int userNotificationId);
    Task<int> CreateNotificationAsync(Notification notification);
    Task<bool> CreateUserNotificationAsync(UserNotification userNotification);
    
}
