using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Cart;
using back.DTOs.Order;
using back.models;
using BackEnd.DTOs.Order;
using Microsoft.AspNetCore.Server.IIS; 
using MongoDB.Driver;

namespace back.services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _order;
        private readonly ICartServices _cartService;

        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;
        public OrderService(IMongoClient client, MongoDbSetting setting, IProductService productService, IInventoryService inventoryService, ICartServices cartServices)
        {
            var db = client.GetDatabase(setting.DatabaseName);
            _order = db.GetCollection<Order>("Orders");
            _productService = productService;
            _inventoryService = inventoryService;
            _cartService = cartServices;
        }


        public async Task<AddOrderResponse> AddOrder(AddOrderDTO data)
        {
            var (orderCheckout, orderProduct) = await this.Checkout(data.Checkout!);
            foreach (var element in orderProduct)
            {
                var modify = await _inventoryService.ReservationInventory(element.Item!.ProductId!, data.Checkout.CartId, element.Item.Quantity);
                if (modify.ModifiedCount == 0) throw new Exception("1 so san pham bi loi please dat hang lai");
            }
            orderCheckout.TotalApplyDiscount += data.FeeShip;
            orderCheckout.FeeShip += data.FeeShip;
            var order = new Order
            {
                OrderAddress = data.Address,
                OrderCheckout = orderCheckout,
                UserId = data.Checkout.UserId,
                OrderItem = orderProduct,
                OrderPayment = data.OrderPayment == PAYMENT.COD.ToString() ? PAYMENT.COD : PAYMENT.ONLINE,
            };
            await _order.InsertOneAsync(order);
            if (!String.IsNullOrEmpty(order.Id))
            {
                foreach (var item in orderProduct)
                {
                    await _cartService.DeleteCart(new DeleteItemDTO
                    {
                        ProductId = item.Item!.ProductId!,
                        UserId = data.Checkout.UserId
                    });

                }
                return new AddOrderResponse
                {
                    IsCreated = true,
                    Id = order.Id,
                    Message = "Order created"
                };
            }
            ;
            return new AddOrderResponse
            {
                IsCreated = false,
                Message = "Order not created"
            }; ;

        }

        public async Task<(OrderCheckout, List<OrderProduct>)> Checkout(CheckoutDTO checkout)
        {
            var cart = _cartService.FindById(checkout.CartId);
            if (cart is null) throw new Exception("cart is not exists");
            decimal totalPrice = 0, totalApplyDiscount = 0, feeShip = 0;
            float amount = 0, totalAmount = 0;
            var items = checkout.Items!;
            List<OrderProduct> listOrder = new List<OrderProduct>();
            foreach (var item in items)
            {
                var product = await _productService.GetProductById(item.ProductId!);
                if (product is null) throw new Exception("product not found");
                var price = product.ProductPrice * item.Quantity;
                totalPrice += price;
                if (product.Discount != 0)
                {
                    amount = (float)item.Quantity * (float)product.ProductPrice * product.Discount / 100;
                    totalAmount += amount;
                }
                listOrder.Add(new OrderProduct
                {
                    TotalPrice = price,
                    TotalApplyDiscount = price - (decimal)amount,
                    Item = new OrderItem
                    {
                        Avatar = product.Avatar,
                        Discount = item.Discount,
                        Price = item.Price,
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        Quantity = item.Quantity
                    }
                });
            }
            totalApplyDiscount = totalPrice - (decimal)totalAmount;
            return (new OrderCheckout
            {
                FeeShip = feeShip,
                TotalApplyDiscount = totalApplyDiscount,
                TotalPrice = totalPrice
            }, listOrder);
        }

        public object DashBoard()
        {
            var orderList = _order.AsQueryable().GroupBy(x => x.OrderStatus).Select(g => new
            {
                Status = g.Key,
                Count = g.Count()
            });
            return orderList;
        }

        public async Task<List<Order>> GetOrder()
        {
            return await _order.Find(x => true).ToListAsync();
        }
        public async Task<Order> GetOrderById(string id)
        {
            return await _order.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task SaveLinkPayment(string id, string link)
        {
            await _order.UpdateOneAsync(x => x.Id == id, Builders<Order>.Update.Set(x => x.LinkPayment, link));
        }

        public async Task<Order> UpdateStatus(UpdateStatusDTO data)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.Id, data.OrderId);
            var update = Builders<Order>.Update.Set(x => x.OrderStatus, OrderState.Confirmed);
            return await _order.FindOneAndUpdateAsync(filter, update);
        }
        public async Task<Order> UpdateStatusPayment(string Id)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.Id , Id);
            var update = Builders<Order>.Update.Set(x => x.OrderStatus,OrderState.Paid);
            return await _order.FindOneAndUpdateAsync(filter, update);
        }
    }
}