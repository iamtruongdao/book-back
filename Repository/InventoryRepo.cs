using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;
using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface IInventoryRepository
    {
        Task<Inventory> Insert(Inventory inventory);
        Task<UpdateResult> Update(FilterDefinition<Inventory> filter, UpdateDefinition<Inventory> update,bool isUpsert);
    }
    public class InventoryRepo : IInventoryRepository
    {
        private readonly IMongoCollection<Inventory> _inventory;
        public InventoryRepo(IMongoClient mongoClient, MongoDbSetting setting)
        {
            var database = mongoClient.GetDatabase(setting.DatabaseName);
            _inventory = database.GetCollection<Inventory>("Inventories");
        }
        public async Task<Inventory> Insert(Inventory inventory)
        {
            await _inventory.InsertOneAsync(inventory);
            return inventory;
        }

        public async Task<UpdateResult> Update(FilterDefinition<Inventory> filter, UpdateDefinition<Inventory> update, bool isUpsert)
        {
            return await _inventory.UpdateOneAsync(filter, update, new UpdateOptions
            {
                IsUpsert = isUpsert,
            }); 
        }
    }
}