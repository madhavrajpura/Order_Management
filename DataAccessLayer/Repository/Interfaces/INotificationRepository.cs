using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface INotificationRepository
{
    List<NotificationViewModel> GetNotificationsById(int UserId);
    Task<bool> MarkNotificationAsRead(int userNotificationId);
    Task<bool> MarkAllNotificationsAsRead(int userId);

    // NEW
    // Added: Methods for creating notification and user notification
    Task<int> CreateNotificationAsync(Notification notification);
    Task<bool> CreateUserNotificationAsync(UserNotification userNotification);

    // Added: Method to check for existing notification request
    Task<bool> HasUserRequestedNotification(int userId, int itemId);

    // Modified: Return NotificationViewModel for pending notifications
    Task<List<NotificationViewModel>> GetPendingStockNotificationsAsync(int itemId);
    
    // // Added: Update user notification
    // Task<bool> UpdateUserNotificationAsync(UserNotification userNotification);
    // Added: Mark original stock notification request as read
    Task<bool> MarkStockNotificationAsReadAsync(int userNotificationId);

}
