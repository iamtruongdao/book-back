using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;

using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface INotificationRepo
    {
        Task<Notification> PushNotification(Notification notification);
        Task<List<Notification>> FindByUser(string userId);
        Task<UpdateResult> UpdateNotificationAsRead(List<string> ids);

    }
    public class NotificationRepo : INotificationRepo
    {
        private readonly IMongoCollection<Notification> _notification;
        public NotificationRepo(IMongoClient client,MongoDbSetting settings)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _notification = database.GetCollection<Notification>("Notifications");
        }

        public async Task<List<Notification>> FindByUser(string userId)
        {
            return await _notification.Find(x => x.ReceiverId == userId).SortByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<Notification> PushNotification(Notification notification)
        {
           await _notification.InsertOneAsync(notification);
            return notification;
        }

        public async Task<UpdateResult> UpdateNotificationAsRead(List<string> ids)
        {
            var filter = Builders<Notification>.Filter.In(x => x.Id, ids);
            var update = Builders<Notification>.Update.Set(x => x.IsRead, true);
            return await _notification.UpdateManyAsync(filter,update);
        }
    }
}