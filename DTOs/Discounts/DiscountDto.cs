using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.DTOs.Discounts
{
    public class DiscountDto
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
         public string? Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Value { get; set; }
        public decimal MinOrderValue { get; set; }
        [BsonRepresentation(BsonType.String)]
        public DiscountType Type { get; set; }
        [BsonRepresentation(BsonType.String)]
        public ApplyTo ApplyTo { get; set; } 
        public bool IsActive { get; set; } 
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string>? UserUsage { get; set; } 
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string>? ProductIds { get; set; }
        public int MaxUsage { get; set; }
        public int UseCount { get; set; }
        public int MaxUsagePerUser { get; set; }
    }
}