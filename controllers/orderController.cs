using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BackEnd.DTOs.Order;
using BackEnd.models;
using BackEnd.services;
using BackEnd.DTOs.VNPay;
using BackEnd.services.VNPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class orderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IVNPayService _vNPayService
        ;
        public orderController(IOrderService orderService, IVNPayService vNPayService)
        {
            _orderService = orderService;
            _vNPayService = vNPayService;
        }

        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout([FromBody] CheckoutDTO data)
        {
            var (checkout, items) = await _orderService.Checkout(data);
            return Ok(new { Code = 0, message = "ok", data = new { checkout, items } });
        }
        [HttpPost("add")]
        public async Task<ActionResult> AddOrder([FromBody] AddOrderDTO data)
        {
            var res = await _orderService.AddOrder(data);
            if (res.IsCreated) return Ok(new { Code = 0, message = res.Message, data = res });
            return Ok(new { Code = 0, message = res.Message });
        }
        [HttpPost("cancel-order")]
        public async Task<ActionResult> CancelOrder([FromBody] UpdateStatusDTO data)
        {
            var res = await _orderService.CancelOrder(data);
           
            return Ok(new { Code = 0, message = "ok" ,data = res});
        }
        [HttpGet("order-statistic")]        
        [Authorize(Roles = nameof(ROLE.Admin))]
        public async Task<ActionResult> OrderStatistic([FromQuery] int year)
        {
            var res = await _orderService.OrderStatistic(year);

            return Ok(new { Code = 0, message = "ok", data = res });
        }
        [HttpGet("user")]
        public async Task<ActionResult> GetOrder()
        {
            var listOrder = await _orderService.GetOrder();
            return Ok(new { Code = 0, data = listOrder, message = "ok" });
        }
        [HttpGet("get-order")]
        public async Task<ActionResult> GetOrderByUserId([FromQuery] OrderState? status)
        {
            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var order = await _orderService.GetOrderByUserId(id,status);
            return Ok(new { Code = 0, data = order, message = "ok" });
        }
         [HttpGet("paginate")]
        public async Task<ActionResult> Pagination([FromQuery] int pageSize,int pageNumber, OrderState? status)
        {
            var order = await _orderService.Filter(pageSize, pageNumber, status);
            return Ok(new { Code = 0, data = order, message = "ok" });
        }
         [HttpGet("get-by-id/{id}")]
        public async Task<ActionResult> GetOrderByUserId(string id)
        {
            var order = await _orderService.GetOrderById(id);
            return Ok(new { Code = 0, data = order, message = "ok" });
        }
        [HttpPost("update-status")]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public async Task<ActionResult> UpdateStatus([FromBody] UpdateStatusDTO id)
        {
            var order = await _orderService.UpdateStatus(id);
            return Ok(new {Code = 0,  data = order, message = "thay doi trang thai thanh cong" });
        }
        [HttpGet("dashboard")]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public ActionResult DashBoard()
        {
            var order = _orderService.DashBoard();
            return Ok(new {Code = 0,message = "ok", data = order });
        }
        [HttpPost("create-payment")]

        public async Task<ActionResult> Payment([FromBody] VNPayRequest request)
        {
            var url = _vNPayService.CreatePayMent(request);
            await _orderService.SaveLinkPayment(request.OrderId!, url);
            return Ok(new {Code = 0 ,message = "ok" ,data = new {url} });
        }
        [HttpGet("payment_callback")]

        public async Task<ActionResult> PaymentCallBack()
        {
            var url = _vNPayService.VNPayExcute(Request.Query);
            await _orderService.UpdateStatusPayment(url.OrderId!);
            return Redirect("http://localhost:5173/order/" + url.OrderId);
        }

    }
}