using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.VNPay
{
    public class VNPayRequest
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? OrderId { get; set; }
        public string? FullName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}