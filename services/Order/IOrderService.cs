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

        Task<(OrderCheckout, List<OrderProduct>)> Checkout(CheckoutDTO checkout);
        Task<AddOrderResponse> AddOrder(AddOrderDTO data);
        Task<Order> UpdateStatus(UpdateStatusDTO id);
        Task<List<Order>> GetOrder();
        Task<Order> GetOrderById(string id);
        Task<Order> UpdateStatusPayment(string id);
        Task SaveLinkPayment(string id, string link);
        List<DashBoardResponse> DashBoard();
        
    }
}