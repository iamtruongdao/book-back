using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Cart
{ 
    public class DeleteItemDTO
    {
        public required string UserId { get; set; }
        public required string ProductId { get; set; }
    }
}