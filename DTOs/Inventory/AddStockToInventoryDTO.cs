using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Inven
{
    public class AddStockToInventoryDTO
    {
        [Required]
        public int Stock { get; set; }
        public string? Location { get; set; } 
        [Required(ErrorMessage = "Product is not null")]
        public string? ProductId { get; set; }
    }
}