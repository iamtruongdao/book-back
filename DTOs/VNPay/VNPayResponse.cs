using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.VNPay
{
    public class VNPayResponse
    {
        public bool Success { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderDescription { get; set; }
        public string? OrderId { get; set; }
        public string? PaymentId { get; set; }
        public string? TransactionId { get; set; }
        public string? Token { get; set; }
        public string? VNPayResponseCode { get; set; }
    }
}