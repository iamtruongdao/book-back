using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Ship
{
    public class ShipResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; } 
        public object? Data { get; set; } 
    }
}