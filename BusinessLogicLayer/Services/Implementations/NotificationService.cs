using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository) => _notificationRepository = notificationRepository;

    public List<NotificationViewModel> GetNotificationsById(int UserId)
    {
        if (UserId <= 0) throw new CustomException("Invalid User ID.");
        return _notificationRepository.GetNotificationsById(UserId);
    }

    public async Task<bool> MarkNotificationAsRead(int userNotificationId)
    {
        if (userNotificationId <= 0) throw new CustomException("Invalid userNotification ID.");
        return await _notificationRepository.MarkNotificationAsRead(userNotificationId);
    }

    public async Task<bool> MarkAllNotificationsAsRead(int userId)
    {
        if (userId <= 0) throw new CustomException("Invalid User ID.");
        return await _notificationRepository.MarkAllNotificationsAsRead(userId);
    }

    // NEW

    public async Task<bool> RequestStockNotificationAsync(int userId, int itemId, string itemName)
    {
        try
        {
            var notification = new Notification
            {
                Message = $"{itemName} is in stock now",
                CreatedBy = userId, // System/admin user for requests
                CreatedAt = DateTime.Now,
                IsActive = false, // Defer activation until stock is available
                ItemId = itemId
            };

            int notificationId = await _notificationRepository.CreateNotificationAsync(notification);
            if (notificationId == 0)
            {
                return false;
            }

            var userNotification = new UserNotification
            {
                UserId = userId,
                NotificationId = notificationId,
                IsRead = false,
                ReadAt = null
            };

            return await _notificationRepository.CreateUserNotificationAsync(userNotification);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> HasUserRequestedNotification(int userId, int itemId)
    {
        return await _notificationRepository.HasUserRequestedNotification(userId, itemId);
    }

    public async Task<List<NotificationViewModel>> GetPendingStockNotificationsAsync(int itemId)
    {
        return await _notificationRepository.GetPendingStockNotificationsAsync(itemId);
    }

    public async Task<bool> MarkStockNotificationAsReadAsync(int userNotificationId)
    {
        return await _notificationRepository.MarkStockNotificationAsReadAsync(userNotificationId);
    }

    public async Task<int> CreateNotificationAsync(Notification notification)
    {
        return await _notificationRepository.CreateNotificationAsync(notification);
    }
    public async Task<bool> CreateUserNotificationAsync(UserNotification userNotification)
    {
        return await _notificationRepository.CreateUserNotificationAsync(userNotification);
    }
}