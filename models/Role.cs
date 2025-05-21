using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace BackEnd.models
{
    [Collection("Roles")]
    public class Role:MongoRole<ObjectId>
    {
        
    }
}