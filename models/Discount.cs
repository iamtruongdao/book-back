using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.models
{
    public class Discount
    {
        [BsonId]
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
        public bool IsActive { get; set; } = true;
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> UserUsage { get; set; } = new List<string>();
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> ProductIds { get; set; } = new List<string>();
        public int MaxUsage { get; set; }
        public int UseCount { get; set; }
        public int MaxUsagePerUser { get; set; }
    }
    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }
    public enum ApplyTo
    {
        All,
        Specific,
    }
}