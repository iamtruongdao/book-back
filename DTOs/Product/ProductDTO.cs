using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using back.DTOs.Author;
using back.models;
using MongoDB.Bson;

namespace back.DTOs.Product
{
    public class ProductDTO
    {
        public string? _id { get; set; }

        public string? ProductName { get; set; }

        public decimal ProductPrice { get; set; }
        public float Discount { get; set; }

        public int ProductQuantity { get; set; }
        public string? ProductDescription { get; set; }
        // public string ProductRating { get; set; } = string.Empty;
        public string? Slug { get; set; } = string.Empty;
        [Required]
        // public int PageNumber { get; set; }
        public string? Translator { get; set; }

        // public DateTime PublicDate { get; set; }
        [Required]
        public string? Avatar { get; set; }
        public string? AuthorName { get; set; }
        public string? Author { get; set; }
        public List<Category>? Cat { get; set; }
        // public bool IsPublic { get; set; }
    }
    public class CreateProductDTO
    {
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    [Required]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;
    [Required]
    public decimal ProductPrice { get; set; }
    [Required]
    public int ProductQuantity { get; set; }
    public float Discount { get; set; }
    public string? ProductDescription { get; set; }
    public string ProductRating { get; set; } = string.Empty;
    public string? Slug { get; set; } = string.Empty;
    [Required]
    public int PageNumber { get; set; }
    public string Translator { get; set; } = string.Empty;
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
        public string? Translator { get; set; } 
        public DateTime PublicDate { get; set; }
        public string? Avatar { get; set; } 
        public string? Author { get; set; }
        public List<string>? Cat { get; set; }
    
    }
}