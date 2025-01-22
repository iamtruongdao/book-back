using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace back.DTOs.Cart
{
    public class IncOrDecProductQuantityDTO
    {
        [Required]
        public string? UserId { get; set; }
        public string?  ProductId  { get; set; }
        public int  OldQuantity  { get; set; }
        public int  Quantity  { get; set; }
    }
}