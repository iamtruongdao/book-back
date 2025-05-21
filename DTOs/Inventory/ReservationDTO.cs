using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Inven
{
    public class ReservationDTO
    {
        [Required]
        public int Quantity { get; set; }
        public DateTime? CreateOn { get; set; } 
        [Required(ErrorMessage = "Product is not null")]
        public string? CartId;
    }
}