using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace back.models
{
    [Collection("Users")]
    public class User:MongoUser<ObjectId>
    {
        
        public string? FullName { get; set; } 
        public string? Slug {get;set;} 
        public override string? Email {get;set;} 
        public string? RefreshToken { get; set; }
        [BsonRepresentation(BsonType.String)]
        public STATUS Status {get;set;} = STATUS.INACTIVE;

    }
    public enum STATUS {
        ACTIVE ,
        INACTIVE
    }
    public enum ROLE {
        User  ,
        Admin
    }
}