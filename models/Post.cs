using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace BackEnd.models
{
    [Collection("Posts")]
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Thumbnail { get; set; }
        public string? Title { get; set; }
        public string? Slug { get; set; }
        public string? Content { get; set; }
        [BsonRepresentation(BsonType.ObjectId)] 
        public string? TagId { get; set; } 

        public DateTime CreatedAt { get; set; } 
        
    }
}