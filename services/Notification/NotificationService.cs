using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Exceptions;
using BackEnd.models;
using BackEnd.Repository;
using BackEnd.services.Notifications;

namespace BackEnd.services.Notifications
{
    public class NotificationService:INotificationService
    {
        private readonly INotificationRepo _notificationRepo;
        public NotificationService(INotificationRepo notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public Task<List<Notification>> GetNotifications(string userId)
        {
            return _notificationRepo.FindByUser(userId);
        }

        public async Task MarkAsRead(List<string> notificationId)
        {

            var result = await _notificationRepo.UpdateNotificationAsRead(notificationId);
            if(result.MatchedCount == 0) throw new BadRequestException("khong tim thay notification nao de update");
        }

        public Task<Notification> PushNotification(string receiverId, string content, string type)
        {
            var notification = new Notification
            {
                ReceiverId = receiverId,
                Content = content,
                Type = type,
             
            };
            return _notificationRepo.PushNotification(notification);
        }
    }
   
}