using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using back.DTOs.Cart;
using back.DTOs.Order;
using back.models;
using BackEnd.DTOs.Order;
using BackEnd.Exceptions;
using BackEnd.Repository;
using BackEnd.services.Ship;
using Microsoft.AspNetCore.Server.IIS; 
using MongoDB.Driver;


namespace back.services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IShipService _shipService;
     

        
        private readonly IInventoryService _inventoryService;
        public OrderService(ICartRepository cartRepo, IInventoryService inventoryService, IOrderRepository orderRepo, IProductRepository productRepo, IShipService shipService)
        {
            _inventoryService = inventoryService;
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _shipService = shipService;
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
            var order = new Order
            {
                OrderAddress = data.Address,
                OrderCheckout = orderCheckout,
                UserId = data.Checkout.UserId,
                OrderItem = orderProduct,
                OrderPayment = data.OrderPayment == PAYMENT.COD.ToString() ? PAYMENT.COD : PAYMENT.VNPAY,
            };
            await _orderRepo.Insert(order);
            if (!String.IsNullOrEmpty(order.Id))
            {
                foreach (var item in orderProduct)
                {
                    await _cartRepo.DeleteCart(data.Checkout.UserId!, item.Item!.ProductId!);
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
            var cart = _cartRepo.FindById(checkout.CartId);
            if (cart is null) throw new NotFoundException("cart is not exists");
            decimal totalPrice = 0, totalApplyDiscount = 0, feeShip = 0;
            float amount = 0, totalAmount = 0;
            var items = checkout.Items!;
            List<OrderProduct> listOrder = new List<OrderProduct>();
            foreach (var item in items)
            {
                var product = await _productRepo.FindById(item.ProductId!);
                if (product is null) throw new BadRequestException("product not found");
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

        public List<DashBoardResponse> DashBoard()
        {
            return _orderRepo.GetOrderStatusCount();
        }

        

        public async Task<List<Order>> GetOrder()
        {
            return await _orderRepo.GetOrder();
        }
        public async Task<Order> GetOrderById(string id)
        {
            return await _orderRepo.GetOrderById(id);
        }

        public async Task SaveLinkPayment(string id, string link)
        {
            await _orderRepo.Update(id, x => x.LinkPayment, link);
        }

        public async Task<Order> UpdateStatus(UpdateStatusDTO data)
        {
            var order = await _orderRepo.GetOrderById(data.OrderId!);
            if(order == null) throw new NotFoundException("Order not found");
            decimal amout = 0;
            if(order.OrderPayment == PAYMENT.COD)
            {
                amout = order.OrderCheckout!.TotalApplyDiscount + order.OrderCheckout.FeeShip;
            }
            var res = await _shipService.createOrder(amout, order.OrderAddress!.FullName!, order.OrderAddress.PhoneNumber!, order.OrderAddress!.Address!, order.OrderAddress!.Street!, order.OrderAddress!.District!, order.OrderAddress.City!, order.OrderCode!);
            if (res.Code == 200)
            {
                await _orderRepo.UpdateStatus(data.OrderId!, OrderState.Confirmed);
            }
            return order;
        }
        public async Task<Order> UpdateStatusPayment(string Id)
        {
            return await _orderRepo.UpdateStatus(Id, OrderState.Paid);

        }
    }
}