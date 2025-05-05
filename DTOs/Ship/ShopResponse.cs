using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Ship
{
    public class ShopResponse
    {
        public int code { get; set; }
        public string? message { get; set; }
        public ShopData? data { get; set; }
    }
      

        public class Shop
        {
            public int _id { get; set; }
            public string? name { get; set; }
            public string? phone { get; set; }
            public string? address { get; set; }
            
        }

        public class ShopData
        {
            public List<Shop>? shops { get; set; }
        }
}
    
