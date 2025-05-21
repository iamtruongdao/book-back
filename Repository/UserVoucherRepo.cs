using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using BackEnd.models;
using MongoDB.Bson;

namespace BackEnd.Repository
{
    public interface IUserDiscountRepository
    {
        Task<UserDiscount> Create(UserDiscount data);
        Task<UserDiscount> FindByDiscountId(string discountId);
        Task<List<BsonDocument>> GetUserVouchers(string userId);
        // Define methods for the repository
    }
    public class UserVoucherRepo :IUserDiscountRepository
    {
        private readonly IMongoCollection<UserDiscount> _userVouchers;
        public UserVoucherRepo(IMongoClient mongoClient, MongoDbSetting setting)
        {
            var database = mongoClient.GetDatabase(setting.DatabaseName);
            _userVouchers = database.GetCollection<UserDiscount>("UserVouchers");
        }

        public async Task<UserDiscount> Create(UserDiscount data)
        {
            await _userVouchers.InsertOneAsync(data);
            return data;
        }

        public Task<UserDiscount> FindByDiscountId(string discountId)
        {
            return _userVouchers.Find(x => x.DiscountId == discountId).FirstOrDefaultAsync();
        }

        public async Task<List<BsonDocument>> GetUserVouchers(string userId)
        {
            return await _userVouchers
                .Aggregate()
                .Match(Builders<UserDiscount>.Filter.Eq(x => x.UserId, userId)).Lookup("Discounts", "DiscountId", "_id", "discounts")
                .ToListAsync();
        }
    }
}