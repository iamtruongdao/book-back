using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

namespace back.DTOs.Product
{
    public class ProductDTO
    {
    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;
    [Required]
    public float ProductPrice { get; set; }
    [Required]
    public int ProductQuantity { get; set; }
    public string? ProductDescription { get; set; }
    public string ProductRating { get; set; } = string.Empty;
    public string? Slug { get; set; } = string.Empty;
    [Required]
    public int PageNumber { get; set; }
    public string Translator { get; set; } = string.Empty;
    [Required]
    public DateTime PublicDate { get; set; }
    [Required]
    public string? Avatar { get; set; } 
    public string? Author { get; set; }
    }
    public class CreateProductDTO
    {
    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;
    [Required]
    public float ProductPrice { get; set; }
    [Required]
    public int ProductQuantity { get; set; }
    public string? ProductDescription { get; set; }
    public string ProductRating { get; set; } = string.Empty;
    public string? Slug { get; set; } = string.Empty;
    [Required]
    public int PageNumber { get; set; }
    public string Translator { get; set; } = string.Empty;
    [Required]
    public DateTime PublicDate { get; set; }
    [Required]
    public string Avatar { get; set; } = string.Empty;
    public string? Author { get; set; }
    }
    public class UpdateProductDTO
    {
    public string? Id { get; set; }
    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;
    public float ProductPrice { get; set; }

    public int ProductQuantity { get; set; }
    public string? ProductDescription { get; set; }
    public string ProductRating { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public string Translator { get; set; } = string.Empty;
    public DateTime PublicDate { get; set; }
    public string? Avatar { get; set; } = string.Empty;
    public string? Author { get; set; }
    }
}