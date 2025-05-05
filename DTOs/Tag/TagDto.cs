using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
    
namespace BackEnd.DTOs.Tag
{
    public class TagDto
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? Name { get; set; }
        public string? Slug { get; set; }
        public int TotalPage { get; set; }
        public DateTime CreatedAt { get; set; } 
        [BsonElement("Tag_Post")]
        public List<Post>? Posts { get; set; }
    }
}