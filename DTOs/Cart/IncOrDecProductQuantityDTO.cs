using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Cart
{
    public class IncOrDecProductQuantityDTO
    {
        [Required]
        public string? UserId { get; set; }
        [Required]
        public string? ProductId { get; set; }
        [Required]
        public int OldQuantity { get; set; }
        [Required]
        
        public int Quantity { get; set; }
    }
}