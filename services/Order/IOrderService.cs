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
        Task<PaginatedList<Order>> GetOrderByUserId(int pageNumber,int pageSize, string? id, OrderState? state,PaymentStatus? paymentStatus);
        Task<Order> GetOrderById(string id);
        Task<Order> UpdateStatusPayment(string id);
        Task SaveLinkPayment(string id, string link);
        Task<PaginatedList<Order>> Filter(int pageSize, int pageNumber, OrderState? state);
        Task<List<DashBoardResponse>> DashBoard();
        Task<Order> CancelOrder(UpdateStatusDTO id);
        Task<List<OrderStatisticResponse>> OrderStatistic(int year);
        
    }
}