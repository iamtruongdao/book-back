using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.DTOs.Cart
{
    public class CartResponse
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public List<CartProductRes>? CartProducts { get; set; }
        public int CartCountProduct { get; set; }
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
        public decimal DiscountPrice => ProductPrice - Discount;
        public string? Avatar { get; set; }
    }
}