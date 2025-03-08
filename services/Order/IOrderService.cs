using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Order;
using back.models;
using BackEnd.DTOs.Order;
namespace back.services
{
    public interface IOrderService
    {
   
        public Task<(OrderCheckout, List<OrderProduct>)> Checkout(CheckoutDTO checkout);
        public Task<AddOrderResponse> AddOrder(AddOrderDTO data);
        public Task<Order> UpdateStatus(UpdateStatusDTO id);
        public Task<List<Order>> GetOrder();
        public Task<Order> GetOrderById(string id);
        public Task<Order> UpdateStatusPayment(string id);
        public Task SaveLinkPayment(string id,string link);
        public object DashBoard();
        
    }
}