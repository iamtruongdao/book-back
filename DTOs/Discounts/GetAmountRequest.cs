using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Order;

namespace BackEnd.DTOs.Discounts
{
    public class GetAmountRequest
    {
        public string? CodeId { get; set; }
        public string? UserId { get; set; }
        public List<OrderItemsDTO>? Items { get; set; }
    }
}