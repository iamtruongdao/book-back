using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.DTOs.Posts
{
    public class PostDto
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Thumbnail { get; set; }
        public string? Title { get; set; }
        public string? Slug { get; set; }
        public string? Content { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? TagId { get; set; }
        public List<Tags> Tag { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}