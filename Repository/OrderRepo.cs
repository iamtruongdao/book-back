
using System.Linq.Expressions;

using BackEnd.models;
using BackEnd.Viewmodel;
using BackEnd.DTOs.Order;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrder();
        Task<Order> GetOrderById(string id);
        Task<List<Order>> FindByState(string user_id, OrderState state);
        Task<List<Order>> FindByUserId(string user_id);
        Task<Order> FindByOrderCode(string orderCode);
        Task<Order> UpdateStatus(string id, OrderState status);
        Task Insert(Order order);
        Task<Order> Update<TField>(string id, Expression<Func<Order, TField>> filed, TField value);
        List<DashBoardResponse> GetOrderStatusCount();
        Task<PaginatedList<Order>> Filter(int pageSize, int pageNumber, FilterDefinition<Order> filter);
        Task<List<BsonDocument>> OrderStatistic(int year);
    }
    public class OrderRepo : IOrderRepository
    {
        private readonly IMongoCollection<Order> _order;
        public OrderRepo(IMongoClient client,MongoDbSetting setting)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _order = database.GetCollection<Order>("Orders");
        }

        public async Task<PaginatedList<Order>> Filter(int pageSize, int pageNumber, FilterDefinition<Order> filter)
        {
            var total = await _order.Find(filter).CountDocumentsAsync();
            var result = await _order.Find(filter).Skip((pageNumber-1)*pageSize).Limit(pageSize).ToListAsync();
            return new PaginatedList<Order>(result,(int)total, pageNumber, pageSize);
        }

        public async Task<Order> FindByOrderCode(string orderCode)
        {
            return await _order.Find(x => x.OrderCode == orderCode).FirstOrDefaultAsync();
        }

        public async Task<List<Order>> FindByState(string user_id,OrderState state)
        {
            return await _order.Find(x => x.UserId == user_id && x.OrderStatus == state).ToListAsync();
        }

        public async Task<List<Order>> FindByUserId(string user_id)
        {
            return await _order.Find(x => x.UserId == user_id).ToListAsync();
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

        public async Task<List<BsonDocument>> OrderStatistic(int yearToFilter)
        {
            // 1. Lấy dữ liệu nhóm theo tháng/năm
            var groupedData = await _order.Aggregate().Match(new BsonDocument
            {
                { "$expr", new BsonDocument("$eq", new BsonArray { new BsonDocument("$year", "$CreatedAt"), yearToFilter }) }
            })
                .Group(new BsonDocument
                {
                    { "_id", new BsonDocument {
                        { "year", new BsonDocument("$year", "$CreatedAt") },
                        { "month", new BsonDocument("$month", "$CreatedAt") }
                    }},
                    { "totalOrders", new BsonDocument("$sum", 1) },
                    { "totalRevenue", new BsonDocument("$sum", "$OrderCheckout.TotalApplyDiscount") }
                })
                .Project(new BsonDocument
                {
                    { "_id", 0 },
                    { "year", "$_id.year" },
                    { "month", "$_id.month" },
                    { "totalOrders", 1 },
                    { "totalRevenue", 1 }
                })
                .ToListAsync();
            return groupedData;
            // 2. Bổ sung các tháng không có dữ liệu
           

        }

        public async Task<Order> Update<TField>(string id, Expression<Func<Order, TField>> filed, TField value)
        {
           return  await _order.FindOneAndUpdateAsync(x => x.Id == id, Builders<Order>.Update.Set(filed, value),new FindOneAndUpdateOptions<Order>
             {
                ReturnDocument = ReturnDocument.After
                
             });
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