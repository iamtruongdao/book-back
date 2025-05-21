using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace BackEnd.models
{
    [Collection("Users")]
    public class User:MongoUser<ObjectId>
    { 
        public string? FullName { get; set; } 
        public string? Slug {get;set;}
        public override string? UserName { get => Email; set {} }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public override string? Email { get; set; }
        public string? RefreshToken { get; set; }
        public string? Otp { get; set; }
        public DateTime OtpExprire { get; set; }
       

    }
    
    public enum ROLE {
        User  ,
        Admin
    }
}