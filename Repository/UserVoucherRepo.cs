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
        Task<UserDiscount> FindByDiscountId(string discountId, string userId);
        Task<(List<BsonDocument>, int)> GetUserVouchers(int pageSize, int pageNumber, string userId);
        Task<DeleteResult> Delete(string id);
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

        public async Task<DeleteResult> Delete(string id)
        {
            return await _userVouchers.DeleteOneAsync(x => x.Id == id);
        }

        public Task<UserDiscount> FindByDiscountId(string discountId,string userId)
        {
            return _userVouchers.Find(x => x.DiscountId == discountId && x.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<(List<BsonDocument>, int)> GetUserVouchers(int pageSize, int pageNumber, string userId)
        {
            var total = await _userVouchers.CountDocumentsAsync(x => x.UserId == userId);
            var result = await _userVouchers
               .Aggregate()
               .Match(Builders<UserDiscount>.Filter.Eq(x => x.UserId, userId)).Lookup("Discounts", "DiscountId", "_id", "discounts").Project(BsonDocument.Parse(@"{Discount:{$arrayElemAt:['$discounts',0]  }}")).Skip((pageNumber - 1) * pageSize).Limit(pageSize)
               .ToListAsync();
            return (result, (int)total);  
        }
    }
}