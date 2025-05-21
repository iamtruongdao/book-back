using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;

namespace BackEnd.DTOs.Order
{
    public class AddOrderDTO
    {
        [Required]
        public OrderAddress? Address { get; set; }
        [Required]
        public string? OrderPayment { get; set; } 
        public decimal FeeShip { get; set; }
        [Required]
        public required CheckoutDTO Checkout { get; set; }
    }
}