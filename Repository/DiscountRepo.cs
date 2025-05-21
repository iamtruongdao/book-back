using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;
using BackEnd.Viewmodel;
using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface IDiscountRepository
    {
        Task<Discount> FindOne(string code);
        Task<Discount> FindById(string id);
        Task<UpdateResult> Update(FilterDefinition<Discount> filter, UpdateDefinition<Discount> update);
        Task<ReplaceOneResult> Replace(Discount discount);
        Task<Discount> CreateDiscount(Discount discount);
        Task<PaginatedList<Discount>> GetAllDiscounts(int pageSize, int pageNumber);
        Task<DeleteResult> DeleteDiscount(string id);
    }
    public class DiscountRepo : IDiscountRepository
    {
        private readonly IMongoCollection<Discount> _discounts;

        public DiscountRepo(IMongoClient mongoClient,MongoDbSetting setting)
        {
            var database = mongoClient.GetDatabase(setting.DatabaseName);
            _discounts = database.GetCollection<Discount>("Discounts");
        }

        public async Task<Discount> CreateDiscount(Discount discount)
        {
            await _discounts.InsertOneAsync(discount);
            return discount;
            
        }

        public async Task<DeleteResult> DeleteDiscount(string id)
        {
            var filter = Builders<Discount>.Filter.Eq(d => d.Id, id);
            return await _discounts.DeleteOneAsync(filter);
        }

        public async Task<Discount> FindById(string id)
        {
            return await _discounts.Find(d => d.Id == id).FirstOrDefaultAsync();
            
        }

        public async Task<Discount> FindOne(string code)
        {
            return await _discounts.Find(d => d.Code == code).FirstOrDefaultAsync();
        }

        public async Task<PaginatedList<Discount>> GetAllDiscounts(int pageSize, int pageNumber)
        {
            var totalCount = await _discounts.CountDocumentsAsync(_ => true);
            var result = await _discounts.Find(_ => true)
               .Skip(pageSize * (pageNumber - 1))
               .Limit(pageSize)
               .ToListAsync();
            return new PaginatedList<Discount>(result, (int)totalCount, pageNumber, pageSize);
        }

        public Task<ReplaceOneResult> Replace(Discount discount)
        {
            return _discounts.ReplaceOneAsync(x => x.Id == discount.Id, discount);
        }

        public Task<UpdateResult> Update(FilterDefinition<Discount> filter, UpdateDefinition<Discount> update)
        {
            return _discounts.UpdateOneAsync(filter, update);
        }
    }
    
}