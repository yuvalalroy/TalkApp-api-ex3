using WebApp.Models;

namespace WebApp.Services
{
    public interface INotificationService
    {
        public Task<Message?> SendNotification(Message message, string username);
        public Task<AndroidDeviceIDModel?> CreateAndoridDeviceOfUser(AndroidDeviceIDModel androidDeviceIDModel, string username);
        public Task<ResponseModel> SendNotification(NotificationModel notificationModel);
    }

}
