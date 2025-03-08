using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.models;

namespace back.DTOs.Order
{
    public class CheckoutDTO
    {
        public required string UserId { get; set; }
        public required string CartId { get; set; }
        public List<OrderItemsDTO>? Items { get; set; }
    }
    public class OrderItemsDTO    
    {
        public string? ProductId { get; set; }
        public decimal Price { get; set; }
        public float Discount { get; set; }
        public int Quantity { get; set; }    
    }
}