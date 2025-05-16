using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.models;

public class Tags
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } 

    public string? Name { get; set; } 
    public string? Slug { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
