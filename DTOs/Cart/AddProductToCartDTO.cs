using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;

namespace BackEnd.DTOs.Cart
{
    
    public class AddProductToCartDTO
    {
        [Required]
        public string? UserId { get; set; }
        public required CartProductItem CartItem  { get; set; }
    }
}