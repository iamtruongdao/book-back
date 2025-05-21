using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;
using BackEnd.DTOs.Ship;
using BackEnd.services.Ship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class shipController : ControllerBase
    {
        private readonly IShipService _shipService;
        public shipController(IShipService shipService)
        {
            _shipService = shipService;
        }
        [HttpPost("fee-ship")]
        public async Task<ActionResult> GetFeeShip([FromBody] object data)
        {
            var fee = await _shipService.getFeeShip(data);
            return Ok(fee);
        }
        [HttpGet("province")]
        public async Task<ActionResult> GetProvince()
        {
            var province = await _shipService.getProvince();
            return Ok(province);
        }
        [HttpGet("district/{provinceId}")]
        public async Task<ActionResult> GetDistrict(string provinceId)
        {
            var district = await _shipService.getDistrict(provinceId);
            return Ok(district);
        }
        [HttpGet("ward/{districtId}")]
        public async Task<ActionResult> GetWard(string districtId)
        {
            var ward = await _shipService.getWard(districtId);
            return Ok(ward);
        }
        [HttpGet("print-shipment")]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public async Task<ActionResult> PrintShipment([FromQuery] string order_code)
        {
            var ward = await _shipService.PrintShipment(order_code);
            return Ok(ward);
        }
        [HttpPost("create-order")]
        [Authorize(Roles = nameof(ROLE.Admin))]

        public async Task<ActionResult> CreateOrder([FromBody] CreateOrderDto data)
        {
            var order = await _shipService.createOrder(
               data.IsPaymentOnline ? 0 : data.Total!.TotalApplyDiscount,                          // amount
               data.Address!.FullName!.Trim(),                  // name
               data.Address!.PhoneNumber!,                  // name
                            // phone
               data.Address.Address!.Trim(),                 // address
               data.Address.Street!.Trim(),              // wardName
               data.Address.District!.Trim(),              // district
               data.Address.City!.Trim(),    // provinceName
               data.OrderCode!,
               data.Time     // orderCode
           );
            return Ok(order);
        }
        [HttpGet("shop-info")]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public async Task<ActionResult> GetShopInfo()
        {
            var order = await _shipService.GetShopInfo(
            );
            return Ok(order);
        }
        [HttpPost("leadtime")]
        public async Task<ActionResult> Leadtime([FromBody] LeadtimeRequest data)
        {
            var order = await _shipService.Leadtime(data);
            return Ok(order);
        }
    }
}