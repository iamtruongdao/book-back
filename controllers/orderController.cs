using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Order;
using back.models;
using back.services;
using BackEnd.DTOs.Order;
using BackEnd.DTOs.VNPay;
using BackEnd.services.VNPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class orderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IVNPayService _vNPayService
        ;
        public orderController(IOrderService orderService,IVNPayService vNPayService)
        {
            _orderService = orderService;
            _vNPayService = vNPayService;
        }
       
        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout([FromBody] CheckoutDTO data)
        {
            try
            {
                var (checkout, items) = await _orderService.Checkout(data);
                return Ok(new { checkout, items });
            }
            catch (System.Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("add")]
        [Authorize]
        public async Task<ActionResult> AddOrder([FromBody] AddOrderDTO data)
        {
            try
            {
                var res = await _orderService.AddOrder(data);
                if (res.IsCreated) return Ok(new {data = res } );
                return BadRequest(new {data = res});
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("user")]
        public async Task<ActionResult> GetOrder()
        {
            var listOrder = await _orderService.GetOrder();
            return Ok(new { data = listOrder, message = "ok" });
        }
        [HttpGet("get/{id}")]
        public async Task<ActionResult> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderById(id);
            return Ok(new { data = order, message = "ok" });
        }
        [HttpPost("update-status")]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public async Task<ActionResult> UpdateStatus([FromBody] UpdateStatusDTO id)
        {
            var order = await _orderService.UpdateStatus(id);
            return Ok(new { data = order, message = "thay doi trang thai thanh cong" });
        }
        [HttpGet("dashboard")]
        // [Authorize(Roles = nameof(ROLE.Admin))]
        public ActionResult DashBoard()
        {
            var order = _orderService.DashBoard();
            return Ok(new { data = order });
        }
        [HttpPost("create-payment")]
       
        public  async Task<ActionResult> Payment([FromBody] VNPayRequest request)
        {
            var url = _vNPayService.CreatePayMent(request);
            await _orderService.SaveLinkPayment(request.OrderId!, url);
            return Ok(new { url });
        }
        [HttpGet("payment_callback")]
       
        public  async Task<ActionResult>  PaymentCallBack()
        {
            var url = _vNPayService.VNPayExcute(Request.Query);
            await _orderService.UpdateStatusPayment(url.OrderId!);
            return Redirect("http://localhost:3000/order/detail/" + url.OrderId);
        }
    }
}