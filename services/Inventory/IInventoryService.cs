using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Inventory;
using back.models;
using MongoDB.Driver;

namespace back.services
{
    public interface IInventoryService
    {
        public Task AddStockToInventory(AddStockToInventoryDTO data);
        public Task<UpdateResult> ReservationInventory(string productId,string cartId,int quantity);
    }
}