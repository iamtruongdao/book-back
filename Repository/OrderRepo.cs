
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
        Task<PaginatedList<Order>> FindByState(int pageNumber,int pageSize,string user_id, OrderState state);
        Task<PaginatedList<Order>> FindByPaymentState(int pageNumber,int pageSize,string user_id, PaymentStatus state);
        Task<PaginatedList<Order>> FindByUserId(int pageNumber,int pageSize, string user_id);
        Task<Order> FindByOrderCode(string orderCode);
        Task<Order> UpdateStatus(string id, OrderState status);
        Task Insert(Order order);
        Task<Order> Update<TField>(string id, Expression<Func<Order, TField>> filed, TField value);
        Task<List<DashBoardResponse>> GetOrderStatusCount();
        Task<PaginatedList<Order>> Filter(int pageSize, int pageNumber, FilterDefinition<Order> filter);
        Task<List<OrderStatisticResponse>> OrderStatistic(FilterDefinition<Order> filter);
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

        public async Task<PaginatedList<Order>> FindByPaymentState(int pageNumber, int pageSize, string user_id, PaymentStatus state)
        {
            var total = _order.Find(x => x.UserId == user_id && x.PaymentStatus == state).CountDocuments();
            var result = await _order.Find(x => x.UserId == user_id && x.PaymentStatus == state)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            return new PaginatedList<Order>(result, (int)total, pageNumber, pageSize);
        }

        public async Task<PaginatedList<Order>> FindByState(int pageNumber,int pageSize,string user_id,OrderState state)
        {
            var total = await _order.Find(x => x.UserId == user_id && x.OrderStatus == state).CountDocumentsAsync();
            var result = await _order.Find(x => x.UserId == user_id && x.OrderStatus == state)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            return new PaginatedList<Order>(result, (int)total, pageNumber, pageSize);
        }

        public async Task<PaginatedList<Order>> FindByUserId(int pageNumber,int pageSize, string user_id)
        {
            var result = await _order.Find(x => x.UserId == user_id)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            var total = await _order.Find(x => x.UserId == user_id).CountDocumentsAsync();
            return new PaginatedList<Order>(result, (int)total, pageNumber, pageSize);
        }

        public async Task<List<Order>> GetOrder()
        {
            return await _order.Find(x => true).ToListAsync();
        }

        public async Task<Order> GetOrderById(string id)
        {
            return await _order.Find(x => x.Id == id).FirstOrDefaultAsync();

        }

        public async Task<List<DashBoardResponse>> GetOrderStatusCount()
        {
            var orderList = await _order.Aggregate()
            .Match(x => x.CreatedAt.Year == DateTime.Now.Year && 
                        x.CreatedAt.Month == DateTime.Now.Month)
            .Group(x => x.OrderStatus, g => new DashBoardResponse
            {
                Status = g.Key.ToString(),
                Count = g.Count()
            })
            .SortByDescending(x => x.Count)
            .ToListAsync();
            return orderList;
        }

        public async Task Insert(Order order)
        {
            await _order.InsertOneAsync(order);
        }

        public async Task<List<OrderStatisticResponse>> OrderStatistic(FilterDefinition<Order> filter)
        {
            // 1. Lấy dữ liệu nhóm theo tháng/năm
            var fluentResult = await _order.Aggregate()
            .Match(filter)
            .Group(x => new { 
                Year = x.CreatedAt.Year, 
                Month = x.CreatedAt.Month 
            }, g => new OrderStatisticResponse{
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalOrders = g.Count(),
                TotalRevenue = g.Sum(x => x.OrderStatus == OrderState.Delivered ? x.OrderCheckout!.TotalApplyDiscount : 0)
            })
            .ToListAsync();
            return fluentResult;

        }

        public async Task<Order> Update<TField>(string id, Expression<Func<Order, TField>> filed, TField value)
        {
           return  await _order.FindOneAndUpdateAsync<Order>(x => x.Id == id, Builders<Order>.Update.Set(filed, value),new FindOneAndUpdateOptions<Order>
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