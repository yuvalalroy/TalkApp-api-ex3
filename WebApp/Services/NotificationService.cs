using CorePush.Google;
using WebApp.Data;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FirebaseAdmin;
using Microsoft.EntityFrameworkCore;
using Message = WebApp.Models.Message;
using WebApp.Services;
using WebApp.Models;
using FirebaseAdmin.Messaging;


namespace WebApp.Service
{

    public class NotificationService : INotificationService
    {
        private readonly WebAppContext _context;
        private readonly IUserService _user;
        public NotificationService(WebAppContext context, IUserService user)
        {
            _context = context;
            _user = user;
        }

        public async Task<Message?> SendNotification(Message message, string username)
        {
            var sent = "false";
            var user = await _user.GetByName(username);
            var checkIfExist = await _context.AndroidDeviceIDModel.Where(item => item.User.userName.Equals(username)).Select(item => item.DeviceId).ToListAsync();
            if (!checkIfExist.Any())
            {
                return null;
            }

            if (message.sent)
            {
                sent = "true";
            }

            ResponseModel response = new ResponseModel();
            try
            {
                var message_tosend = new MulticastMessage()
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "contactID", message.Contact.id },
                        { "content", message.content },
                        { "created", message.created },
                    },

                    Notification = new Notification
                    {
                        Title = message.Contact.id,
                        Body = message.content
                    },

                    Tokens = checkIfExist
                };

                var messaging = FirebaseMessaging.DefaultInstance;
                var fcmSendResponse = await messaging.SendMulticastAsync(message_tosend);
                return message;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<AndroidDeviceIDModel?> CreateAndoridDeviceOfUser(AndroidDeviceIDModel androidDeviceIDModel, string username)
        {
            var user = await _user.GetByName(username);
            androidDeviceIDModel.User = user;
            var checkIfExist = await _context.AndroidDeviceIDModel.FirstOrDefaultAsync(item => item.DeviceId.Equals(androidDeviceIDModel.DeviceId));
            if (checkIfExist != null)
            {
                _context.AndroidDeviceIDModel.Remove(checkIfExist);
            }

            _context.AndroidDeviceIDModel.Add(androidDeviceIDModel);
            await _context.SaveChangesAsync();
            return androidDeviceIDModel;
        }

        public Task<ResponseModel> SendNotification(NotificationModel notificationModel)
        {
            throw new NotImplementedException();
        }
    }
}