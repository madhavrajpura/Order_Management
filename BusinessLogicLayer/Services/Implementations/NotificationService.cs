using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository) => _notificationRepository = notificationRepository;

    public List<NotificationViewModel> GetNotificationsById(int UserId) => _notificationRepository.GetNotificationsById(UserId);

    public async Task<bool> MarkNotificationAsRead(int userNotificationId) => await _notificationRepository.MarkNotificationAsRead(userNotificationId);

    public async Task<bool> MarkAllNotificationsAsRead(int userId) => await _notificationRepository.MarkAllNotificationsAsRead(userId);

}
