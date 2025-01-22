using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace back.models
{
    [Collection("Authors")]
    public class Author
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? Avatar {get;set;} 
        public string? AuthorDescription { get; set; } 
        public string? Slug {get;set;}
    }
   
   
}