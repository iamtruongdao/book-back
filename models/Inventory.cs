using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackEnd.models
{
    public class Inventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int Stock { get; set; }
        public string Location { get; set; } = "unKnown";
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }
        public List<InvenReservation> Reservation { get; set; } = new List<InvenReservation>();
    }

    public class InvenReservation
    {
        public int Quantity { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CartId { get; set; }
        public DateTime CreateOn { get; set; }
    }
}