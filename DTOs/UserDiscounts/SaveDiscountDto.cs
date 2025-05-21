using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.UserDiscounts
{
    public class SaveDiscountDto
    {
        public string? DiscountId { get; set; }
        public string? UserId { get; set; }
    }
}