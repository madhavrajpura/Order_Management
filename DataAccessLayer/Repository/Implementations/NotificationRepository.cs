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
}
