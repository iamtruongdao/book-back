using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using back.models;
using BackEnd.DTOs.Order;
using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrder();
        Task<Order> GetOrderById(string id);
        Task<Order> UpdateStatus(string id, OrderState status);
        Task Insert(Order order);
        Task Update<TField>(string id,Expression<Func<Order,TField>> filed,TField value);
        List<DashBoardResponse> GetOrderStatusCount();
    }
    public class OrderRepo : IOrderRepository
    {
        private readonly IMongoCollection<Order> _order;
        public OrderRepo(IMongoClient client,MongoDbSetting setting)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _order = database.GetCollection<Order>("Orders");
        }
        public async Task<List<Order>> GetOrder()
        {
            return await _order.Find(x => true).ToListAsync();
        }

        public async Task<Order> GetOrderById(string id)
        {
            return await _order.Find(x => x.Id == id).FirstOrDefaultAsync();

        }

        public List<DashBoardResponse> GetOrderStatusCount()
        {
            var orderList = _order.AsQueryable().GroupBy(x => x.OrderStatus).Select(g => new
            DashBoardResponse
            {
                Status = g.Key.ToString(),
                Count = g.Count()
            }).ToList();
            return orderList;
        }

        public async Task Insert(Order order)
        {
            await _order.InsertOneAsync(order);
        }

        public async Task Update<TField>(string id, Expression<Func<Order, TField>> filed, TField value)
        {
             await _order.UpdateOneAsync(x => x.Id == id, Builders<Order>.Update.Set(filed, value));
        }

        public async Task<Order> UpdateStatus(string id, OrderState status)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.Id, id);
            var update = Builders<Order>.Update.Set(x => x.OrderStatus, status);
            return await _order.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Order>
            {
                ReturnDocument = ReturnDocument.After
            });
        }
    }
}