using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;

namespace BackEnd.DTOs.Ship
{
    public class CreateOrderDto
    {
        public OrderCheckout? Total { get; set; }
        public OrderAddress? Address { get; set; }
        public string? OrderCode { get; set; }
        public long Time { get; set; }
        public bool IsPaymentOnline { get; set; }
    }
}