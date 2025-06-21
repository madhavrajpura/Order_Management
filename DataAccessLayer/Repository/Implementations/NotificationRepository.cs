using DataAccessLayer.Context;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _db;

    public NotificationRepository(ApplicationDbContext db) => _db = db;

    public List<NotificationViewModel> GetNotificationsById(int UserId)
    {
        var data = _db.UserNotifications
                    .Include(un => un.Notification)
                    .Where(n => n.UserId == UserId && !n.IsRead && n.Notification.IsActive)
                    .Select(n => new NotificationViewModel
                    {
                        UserNotificationId = n.Id,
                        NotificationId = n.NotificationId,
                        UserId = n.UserId,
                        IsRead = n.IsRead,
                        Message = n.Notification.Message,
                        IsActive = n.Notification.IsActive
                    }).ToList();

        if (data == null)
        {
            return new List<NotificationViewModel>();
        }
        return data;
    }

    public async Task<bool> MarkNotificationAsRead(int userNotificationId)
    {
        var notification = await _db.UserNotifications.FindAsync(userNotificationId);
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
        var notifications = _db.UserNotifications
            .Where(n => n.UserId == userId && !n.IsRead);
        if (!notifications.Any())
        {
            return false;
        }

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
        }
        await _db.SaveChangesAsync();
        return true;
    }

}