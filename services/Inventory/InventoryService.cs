using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using back.DTOs.Inventory;
using back.models;
using MongoDB.Driver;

namespace back.services
{
    public class InventoryService:IInventoryService
    {
        private readonly IMongoCollection<Inventory> _inventory;
        private readonly IMapper _mapper;
        public InventoryService(IMongoClient client, MongoDbSetting setting, IMapper mapper)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _inventory = database.GetCollection<Inventory>("Inventories");
            _mapper = mapper;
        }

        public async Task AddStockToInventory(AddStockToInventoryDTO data)
        {
            await _inventory.InsertOneAsync(_mapper.Map<Inventory>(data));  
        }

        public async Task<UpdateResult> ReservationInventory(string productId, string cartId, int quantity)
        {
            var builder = Builders<Inventory>.Filter;
            var filter = builder.And(builder.Eq(x => x.ProductId,productId),builder.Gte(x => x.Stock, quantity));
            var update = Builders<Inventory>.Update.Inc("Stock",-quantity).AddToSet("Reservation",new InvenReservation
            {
                CartId = cartId,
                Quantity = quantity,
                CreateOn =  DateTime.Now
            });
            return await _inventory.UpdateOneAsync(filter, update, new UpdateOptions
            {
                IsUpsert = true,
            }); 
        }
    }
}