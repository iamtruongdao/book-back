using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Inven;
using MongoDB.Driver;

namespace BackEnd.services
{
    public interface IInventoryService
    {
        public Task AddStockToInventory(AddStockToInventoryDTO data);
        public Task<UpdateResult> ReservationInventory(string productId,string cartId,int quantity);
    }
}