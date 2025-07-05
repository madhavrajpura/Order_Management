using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class NotificationRepository : INotificationRepository
{
    private readonly NewItemOrderDbContext _db;

    public NotificationRepository(NewItemOrderDbContext db) => _db = db;

    public List<NotificationViewModel> GetNotificationsById(int UserId)
    {
        List<NotificationViewModel>? data = _db.UserNotifications
                    .Include(un => un.Notification)
                    .Where(n => n.UserId == UserId && !n.IsRead && (bool)n.Notification.IsActive)
                    .Select(n => new NotificationViewModel
                    {
                        UserNotificationId = n.Id,
                        NotificationId = n.NotificationId,
                        UserId = n.UserId,
                        IsRead = n.IsRead,
                        ItemId = n.Notification.ItemId,
                        Message = n.Notification.Message,
                        IsActive = (bool)n.Notification.IsActive
                    }).ToList();

        if (data == null)
        {
            return new List<NotificationViewModel>();
        }
        return data;
    }

    public async Task<bool> MarkNotificationAsRead(int userNotificationId)
    {
        UserNotification? notification = await _db.UserNotifications.FindAsync(userNotificationId) ?? throw new Exception();
        if (notification != null)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> MarkAllNotificationsAsRead(int userId)
    {
        IQueryable<UserNotification>? notifications = _db.UserNotifications
            .Where(n => n.UserId == userId && !n.IsRead) ?? throw new Exception();
        if (!notifications.Any())
        {
            return false;
        }

        foreach (UserNotification? notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
        }
        await _db.SaveChangesAsync();
        return true;
    }

    // NEW
    // Added: Create a notification entry
    public async Task<int> CreateNotificationAsync(Notification notification)
    {
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
        return notification.Id;
    }

    // Added: Create a user notification entry
    public async Task<bool> CreateUserNotificationAsync(UserNotification userNotification)
    {
        _db.UserNotifications.Add(userNotification);
        await _db.SaveChangesAsync();
        return true;
    }

    // Added: Check if user has already requested notification for an item
    public async Task<bool> HasUserRequestedNotification(int userId, int itemId)
    {
        return await _db.UserNotifications
            .Join(_db.Notifications,
                un => un.NotificationId,
                n => n.Id,
                (un, n) => new { un, n })
            .AnyAsync(x => x.un.UserId == userId &&
                          x.n.Message.Contains($"ItemId:{itemId}") && // Assuming message includes ItemId for uniqueness
                          !x.un.IsRead &&
                          (bool)x.n.IsActive);
    }

    public async Task<List<NotificationViewModel>> GetPendingStockNotificationsAsync(int itemId)
    {
        return await _db.UserNotifications
            .Join(_db.Notifications,
                un => un.NotificationId,
                n => n.Id,
                (un, n) => new { un, n })
            .Where(x => x.n.ItemId == itemId && !x.un.IsRead && (bool)!x.n.IsActive)
            .Select(x => new NotificationViewModel
            {
                NotificationId = x.n.Id,
                Message = x.n.Message,
                CreatedBy = x.n.CreatedBy,
                IsActive = (bool)x.n.IsActive,
                UserNotificationId = x.un.Id,
                UserId = x.un.UserId,
                IsRead = x.un.IsRead,
                ReadAt = x.un.ReadAt,
                ItemId = x.n.ItemId
            })
            .ToListAsync();
    }

    // public async Task<bool> UpdateUserNotificationAsync(UserNotification userNotification)
    // {
    //     _db.UserNotifications.Update(userNotification);
    //     await _db.SaveChangesAsync();
    //     return true;
    // }

    public async Task<bool> MarkStockNotificationAsReadAsync(int userNotificationId)
    {
        var userNotification = await _db.UserNotifications
            .FirstOrDefaultAsync(un => un.Id == userNotificationId);
        if (userNotification != null)
        {
            userNotification.IsRead = true;
            userNotification.ReadAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }
        return false;
    }

}