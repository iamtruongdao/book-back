using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Discounts
{
    public class UpdateDiscountDto :CreateDiscountDto
    {
        public string? Id { get; set; }
    }
}