
using AutoMapper;
using BackEnd.DTOs.Inven;
using BackEnd.models;
using BackEnd.Repository;
using MongoDB.Driver;

namespace BackEnd.services
{
    public class InventoryService:IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;
       
        private readonly IMapper _mapper;
        public InventoryService(IInventoryRepository inventoryRepo,IMapper mapper)
        {
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
        }
        
        public async Task AddStockToInventory(AddStockToInventoryDTO data)
        {
            await _inventoryRepo.Insert(_mapper.Map<Inventory>(data));
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
            return await _inventoryRepo.Update(filter,update,true);
        }
    }
}