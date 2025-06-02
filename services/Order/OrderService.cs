using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using BackEnd.DTOs.Order;
using BackEnd.models;
using BackEnd.Viewmodel;
using BackEnd.DTOs.Discounts;

using BackEnd.DTOs.Ship;
using BackEnd.Exceptions;
using BackEnd.hub;

using BackEnd.Repository;
using BackEnd.services.Discounts;
using BackEnd.services.Ship;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Microsoft.Extensions.Caching.Memory;


namespace BackEnd.services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IShipService _shipService;
        private readonly INotificationRepo _notificationRepo;
        private readonly IInventoryService _inventoryService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IDiscountService _discountService;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IMemoryCache _cache;
        public OrderService(ICartRepository cartRepo, IInventoryService inventoryService, IOrderRepository orderRepo, IProductRepository productRepo, IShipService shipService, IHubContext<NotificationHub> hubContext, INotificationRepo notificationRepo, IDiscountService discountService, IMemoryCache cache, IInventoryRepository inventoryRepo)
        {
            _notificationRepo = notificationRepo;
            _hubContext = hubContext;
            _cartRepo = cartRepo;
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _shipService = shipService;
            _inventoryService = inventoryService;
            _discountService = discountService;
            _cache = cache;
            _inventoryRepo = inventoryRepo;
        }
        public async Task<AddOrderResponse> AddOrder(AddOrderDTO data)
        {
            var (orderCheckout, orderProduct) = await this.Checkout(data.Checkout!);
            foreach (var element in orderProduct)
            {
                var modify = await _inventoryService.ReservationInventory(element.Item!.ProductId!, data.Checkout.CartId, element.Item.Quantity);
                if (modify.ModifiedCount == 0) throw new BadRequestException("1 so san pham bi loi please dat hang lai");
            }
            orderCheckout.FeeShip += data.FeeShip;
            var random = new Random();
            string randomPart = new string(Enumerable.Range(0, 6)
            .Select(_ => (char)random.Next('A', 'Z' + 1)).ToArray());
            var deleveryTime = await _shipService.Leadtime(new LeadtimeRequest {
               FromWardCode = "1B2808",
                ToDistrictId = int.Parse(data.Address!.District!),
                FromDistrictId = 3255,
                ToWardCode = data.Address.Street,
                ServiceId = 53320
            });
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(deleveryTime.Data!.Leadtime * 1000);
            var order = new Order
            {
                OrderAddress = data.Address,
                OrderCheckout = orderCheckout,
                UserId = data.Checkout.UserId,
                OrderItem = orderProduct,
                OrderPayment = data.OrderPayment == PAYMENT.COD.ToString() ? PAYMENT.COD : PAYMENT.VNPAY,
                PaymentStatus = data.OrderPayment != PAYMENT.COD.ToString() ? PaymentStatus.WaitingPaid : null,
                OrderCode = $"ORD{DateTime.UtcNow:yyyyMMdd}-{randomPart}",
                DeleveredAt = dateTimeOffset.UtcDateTime,

            };
            if (data.Checkout.Vouchers.Count > 0)
            {
                var result = await _discountService.UpdateUserUse(data.Checkout.UserId!, data.Checkout.Vouchers[0]);
                if(result.MatchedCount == 0) throw new NotFoundException("error please try again");
            }
            await _orderRepo.Insert(order);
            if (!String.IsNullOrEmpty(order.Id))
            {
                foreach (var item in orderProduct)
                {
                    await _cartRepo.DeleteCart(data.Checkout.UserId!, item.Item!.ProductId!);
                }
                _cache.Remove($"cart_price_${data.Checkout.UserId}");
                return new AddOrderResponse
                {
                    IsCreated = true,
                    Id = order.Id,
                    Message = "Order created"
                };
            }
            
            return new AddOrderResponse
            {
                IsCreated = false,
                Message = "Order not created"
            }; 

        }

        public async Task<Order> CancelOrder(UpdateStatusDTO data)
        {
            if (data.OrderId == null) throw new NotFoundException("đã xảy ra lỗi k tìm thấy đơn hàng");
            var order = await _orderRepo.GetOrderById(data.OrderId);
            if (order == null) throw new NotFoundException("đã xảy ra lỗi k tìm thấy đơn hàng");
            foreach (var item in order.OrderItem!)
            {
                var quantity = item.Item!.Quantity!;
                if (quantity > 0)
                {
                    var modify = await _inventoryRepo.Refund(item.Item!.ProductId!, quantity);
                    if (modify.ModifiedCount == 0) throw new BadRequestException("refund inventory failed");
                }
            }
            return await _orderRepo.Update(data.OrderId, x => x.OrderStatus, OrderState.Cancel);
        }

        public async Task<(OrderCheckout, List<OrderProduct>)> Checkout(CheckoutDTO checkout)
        {
            var cart = _cartRepo.FindById(checkout.CartId);
            if (cart is null) throw new NotFoundException("cart is not exists");
            var cacheKey = $"cart_price_{checkout.UserId}";
            var userPriceDict = _cache.Get<Dictionary<string, decimal>>(cacheKey);
            var totalCheckout = new OrderCheckout
            {
                FeeShip = 0,
                TotalPrice = 0,
                VoucherDiscount = 0,
                TotalApplyDiscount = 0,
            } ;
            decimal totalPrice = 0, totalApplyDiscount = 0;
            float  totalAmount = 0;
            var items = checkout.Items!;
            List<OrderProduct> listOrder = new List<OrderProduct>();
            foreach (var item in items)
            {
                float amount = 0;
                var product = await _productRepo.FindById(item.ProductId!);
                if (product is null) throw new BadRequestException("product not found");
                var quantity = item.Quantity;
                var unitPrice = product.ProductPrice;
                if (userPriceDict != null && userPriceDict.TryGetValue(item.ProductId!, out decimal lockedPrice))
                {
                    unitPrice = lockedPrice;
                }
                var itemTotalPrice = unitPrice * quantity;
                totalPrice += itemTotalPrice;
                if (product.Discount != 0)
                {
                    amount = (float)item.Quantity * (float)unitPrice * product.Discount / 100;//giảm giá trên từng  sản phẩm
                    totalAmount += amount;
                }
                listOrder.Add(new OrderProduct
                {
                    TotalPrice = itemTotalPrice,
                    TotalApplyDiscount = itemTotalPrice - (decimal)amount,
                    Item = new OrderItem
                    {
                        Avatar = product.Avatar,
                        Discount = amount,
                        Price = unitPrice,
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        Quantity = item.Quantity
                    }
                });
            }
            if (checkout.Vouchers.Count > 0)
            {
                foreach (var voucher in checkout.Vouchers)
                {
                    var discount = await _discountService.GetAmount(new GetAmountRequest
                    {
                        CodeId = voucher,
                        UserId = checkout.UserId,
                        Items = items
                    });
                    if (discount is null) throw new NotFoundException("voucher not found");
                    if (discount.Amount > 0)
                    {
                        totalCheckout.VoucherDiscount += discount.Amount;
                    }
                }
            }
            totalApplyDiscount = totalPrice - (decimal)totalAmount;
            totalCheckout.TotalPrice = totalPrice;
            totalCheckout.TotalApplyDiscount = totalApplyDiscount - totalCheckout.VoucherDiscount;
            return (totalCheckout, listOrder);
        }

        public async Task<List<DashBoardResponse>> DashBoard()
        {
            return await _orderRepo.GetOrderStatusCount();
        }

        public async Task<PaginatedList<Order>> Filter(int pageSize, int pageNumber, OrderState? state)
        {
            var builderFilter = Builders<Order>.Filter;
            var filter = builderFilter.Empty;
            if (state != null) filter = builderFilter.Eq(x => x.OrderStatus, state);
            return await _orderRepo.Filter(pageSize, pageNumber, filter);
        }

        public async Task<List<Order>> GetOrder()
        {
            return await _orderRepo.GetOrder();
        }

        public async Task<Order> GetOrderById(string id)
        {
            return await _orderRepo.GetOrderById(id);
        }

        public async Task<PaginatedList<Order>> GetOrderByUserId(int pageNumber, int pageSize, string? id, OrderState? state = null, PaymentStatus? paymentStatus = null)
        {
            if (id == null) throw new UnAuthorizeException("please login to continue");
            PaginatedList<Order> result;
            if (state.HasValue)
            {
                result = await _orderRepo.FindByState(pageNumber, pageSize, id, state.Value);
            }
            else if (paymentStatus.HasValue)
            {
                result = await _orderRepo.FindByPaymentState(pageNumber, pageSize, id, paymentStatus.Value);
            }
            else
            {
                // Lấy tất cả orders
                result = await _orderRepo.FindByUserId(pageNumber, pageSize, id);
            }
            return result;
        }

        public async Task<List<OrderStatisticResponse>> OrderStatistic(int year)
        {
            var filter = Builders<Order>.Filter.Where(x => x.CreatedAt.Year == year);
            var result = await _orderRepo.OrderStatistic(filter);
            var fullYearData = Enumerable.Range(1, 12).Select(month =>
            {
                var existing = result.FirstOrDefault(x =>
                    x.Year == year && x.Month == month);

                return new OrderStatisticResponse
                {
                    Year = year,
                    Month = month,
                    TotalOrders = existing?.TotalOrders ?? 0,
                    TotalRevenue = existing?.TotalRevenue ?? 0
                };
            }).ToList();
            
            return fullYearData;
        }
        
        public async Task SaveLinkPayment(string id, string link)
        {
            await _orderRepo.Update(id, x => x.LinkPayment, link);
        }

        public async Task<Order> UpdateStatus(UpdateStatusDTO data)
        {
            var order = await _orderRepo.GetOrderById(data.OrderId!);
            if(order == null) throw new NotFoundException("Order not found");
            await _orderRepo.UpdateStatus(order.Id!, OrderState.WaitingPickup);
            var notify = new Notification
            {
                Content = $"Đơn hàng {order.OrderItem![0].Item!.ProductName} của bạn đã được xác nhận",
                ReceiverId = order.UserId!,
                Type = "message",
            };
            await _notificationRepo.PushNotification(notify);
            await _hubContext.Clients.User(order.UserId!).SendAsync("Send", notify);
            // }
            return order;
        }
        public async Task<Order> UpdateStatusPayment(string Id)
        {
            return await _orderRepo.Update(Id, x => x.PaymentStatus,PaymentStatus.Paid);
        }
    }
}