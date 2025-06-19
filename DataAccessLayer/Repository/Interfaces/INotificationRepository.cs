using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface INotificationRepository
{
    List<NotificationViewModel> GetNotificationsById(int UserId);
    ////////////////////////////////////////////////////////
    Task<bool> MarkNotificationAsRead(int userNotificationId);
    Task<bool> MarkAllNotificationsAsRead(int userId);
}
