using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Order
{
    public class AddOrderResponse
    {
        public bool IsCreated { get; set; }
        public string? Id { get; set; }
        public string? Message { get; set; }
    }
}