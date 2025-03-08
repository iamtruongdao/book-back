using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.models;

namespace back.DTOs.Cart
{
    public class CartResponse
    {
        public string? _id { get; set; }
        public string? UserId { get; set; }
        public List<CartProductRes>? CartProduct { get; set; }
    }
    public class CartProductRes : CartProductItem
    {
        public ProductDetail? ProductDetails { get; set; }
    }
    public class ProductDetail {
        public string? ProductName { get; set; }
        public string? Slug { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal Discount { get; set; }
        public string? Avatar { get; set; }
    }
}