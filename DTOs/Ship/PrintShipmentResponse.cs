using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Ship
{
    public class PrintShipmentResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public Data? Data { get; set; }
    }
    public class Data
    {
        public string? Token { get; set; }
    }
}