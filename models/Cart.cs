using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace back.models
{
    [Collection("Carts")]
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }
        public List<CartProductItem>? CartProduct { get; set; }
        public int CartCountProduct { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class CartProductItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
    }
}