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

namespace back.models
{   

    [Collection("Products")]
    public class Product
    {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } 
     public string ProductName { get; set; } = string.Empty;
    public float ProductPrice { get; set; }
    public int ProductQuantity { get; set; }
    public string? ProductDescription { get; set; }
    public string ProductRating { get; set; } = string.Empty;
    public string? Slug { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public string Translator { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.DateTime)]
    public DateTime PublicDate { get; set; }

    [BsonRepresentation(BsonType.String)]
    public bool IsPublic { get; set; } = false;

    public string Avatar { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? Author { get; set; }
    }
   
}