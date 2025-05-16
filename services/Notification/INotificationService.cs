using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;

namespace BackEnd.services.Notifications
{
    public interface INotificationService
    {
        Task<Notification> PushNotification(string receiverId, string content, string type);
        Task<List<Notification>> GetNotifications(string userId);
        Task MarkAsRead(List<string> notificationId);
    }
}