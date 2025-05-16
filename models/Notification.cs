using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? SenderId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ReceiverId { get; set; }
        public string? Content { get; set; }
        public bool IsRead { get; set; } = false;
        public string? Type { get; set; } // "message", "friend_request", "like", etc.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}