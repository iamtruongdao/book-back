using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using BackEnd.DTOs.Author;
using BackEnd.models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.DTOs.Product
{
    public class ProductDTO
    {
        
       [BsonElement("_id")]
       [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
 
        public string? ProductName { get; set; }

        public decimal ProductPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public float Discount { get; set; }

        public int ProductQuantity { get; set; }
        public int Sold { get; set; }
        public string? ProductDescription { get; set; }
        // public string ProductRating { get; set; } 
        public string? Slug { get; set; }
        [Required]
        public int PageNumber { get; set; }
        public string? Translator { get; set; }

        public DateTime PublicDate { get; set; }
        [Required]
        public string? Avatar { get; set; }
        public string? AuthorName { get; set; }
        public string? Author { get; set; }
        public List<Category>? Category { get; set; }
        public bool IsPublic { get; set; }
    }
    public class CreateProductDTO
    {
    public string? Id { get; set; } 
    [Required]
    [StringLength(200)]
    public string? ProductName { get; set; } 
    [Required]
    public decimal ProductPrice { get; set; }
    [Required]
    public int ProductQuantity { get; set; }
    public float Discount { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductRating { get; set; } 
    public string? Slug { get; set; } 
    [Required]
    public int PageNumber { get; set; }
    public string? Translator { get; set; } 
    [Required]
    public DateTime PublicDate { get; set; }

    public List<string>? Cat { get; set; }
    [Required]
    public string? Avatar { get; set; } 
    public string? Author { get; set; }
    }
    public class UpdateProductDTO
    {
        public string? Id { get; set; }
        [Required]
        [StringLength(100)]
        public string? ProductName { get; set; } 
        public string? Slug { get; set; } 
        public float ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public float Discount { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductRating { get; set; } 
        public int PageNumber { get; set; }
        public string? Translator { get ; set; } 
        public DateTime PublicDate { get; set; }
        public string? Avatar { get; set; } 
        public string? Author { get; set; }
        public List<string>? Cat { get; set; }
    
    }
}