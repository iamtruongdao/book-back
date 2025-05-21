using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public OrderCheckout? OrderCheckout { get; set; }
        public OrderAddress? OrderAddress { get; set; }
        public List<OrderProduct>? OrderItem { get; set; }
        [BsonRepresentation(BsonType.String)]
        public OrderState OrderStatus { get; set; } = OrderState.Pending;
        [BsonRepresentation(BsonType.String)]
        public PAYMENT OrderPayment { get; set; }
        public string? OrderCode { get; set; }
        public string? TrackingNumber { get; set; }
        [BsonRepresentation(BsonType.String)]

        public PaymentStatus? PaymentStatus { get; set; }
        public string? LinkPayment { get; set; }
        public DateTime? DeleveredAt { get; set; }    
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public class OrderAddress {
        public string? Street { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

    }
    public class OrderItem {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Avatar { get; set; }
        public decimal Price { get; set; }
        public float Discount { get; set; }
        public int Quantity { get; set; }
    }
    public class OrderProduct
    {
        public decimal TotalPrice { get; set; }
        public decimal TotalApplyDiscount { get; set; }
        public OrderItem? Item { get; set; }
    }
    public class OrderCheckout
    {
        public decimal TotalPrice { get; set; }
        public decimal VoucherDiscount { get; set; }
        public decimal TotalApplyDiscount { get; set; }
        public decimal FeeShip { get; set; }
    }
    public enum PAYMENT
    {
        COD,
        VNPAY
    }
    public enum PaymentStatus
    {
        Paid,
        WaitingPaid,
        Refund,
        Failed
    }
    public enum OrderState      
    {
        Pending,
        WaitingPickup,
        Shipping,
        Cancel,
        Delivered,
       
    
    }
}