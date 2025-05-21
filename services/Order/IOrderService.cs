using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Order;
using BackEnd.models;
using BackEnd.Viewmodel;

namespace BackEnd.services
{
    public interface IOrderService
    {

        Task<(OrderCheckout, List<OrderProduct>)> Checkout(CheckoutDTO checkout);
        Task<AddOrderResponse> AddOrder(AddOrderDTO data);
        Task<Order> UpdateStatus(UpdateStatusDTO id);
        Task<List<Order>> GetOrder();
        Task<List<Order>> GetOrderByUserId(string? id, OrderState? state);
        Task<Order> GetOrderById(string id);
        Task<Order> UpdateStatusPayment(string id);
        Task SaveLinkPayment(string id, string link);
        Task<PaginatedList<Order>> Filter(int pageSize, int pageNumber, OrderState? state);
        List<DashBoardResponse> DashBoard();
        Task<Order> CancelOrder(UpdateStatusDTO id);
        Task<List<OrderStatisticResponse>> OrderStatistic(int year);
        
    }
}