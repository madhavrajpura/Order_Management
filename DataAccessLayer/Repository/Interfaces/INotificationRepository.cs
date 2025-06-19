using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface INotificationRepository
{
    List<NotificationViewModel> GetNotificationsById(int UserId);
}
