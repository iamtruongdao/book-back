using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Discounts
{
    public class GetAmountResponse
    {
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal FinalPrice { get; set; }
    }
}