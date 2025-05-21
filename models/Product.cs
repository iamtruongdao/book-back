using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.models
{

    [Collection("Products")]
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? ProductName { get; set; } 
        public decimal ProductPrice { get; set; }
        public float Discount { get; set; }
        public int ProductQuantity { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductRating { get; set; } 
        public string? Slug { get; set; } 
        public int PageNumber { get; set; }
        public string Translator { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime PublicDate { get; set; }

        [BsonRepresentation(BsonType.Boolean)]

        public bool IsPublic { get; set; } = false;
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string>? Cat { get; set; }
        public int Sold { get; set; }
        public string? Avatar { get; set; } 

        [BsonRepresentation(BsonType.ObjectId)]
        public string? Author { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
   
}