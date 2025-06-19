using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface INotificationService
{
    List<NotificationViewModel> GetNotificationsById(int UserId);
    Task<bool> MarkNotificationAsRead(int userNotificationId);
    Task<bool> MarkAllNotificationsAsRead(int userId);
}
