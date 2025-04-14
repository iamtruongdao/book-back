using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.services.Ship;
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

        [HttpPost("create-order")]
        public async Task<ActionResult> CreateOrder([FromBody] object data)
        {
            var order = await _shipService.createOrder(
                250000,                          // amount
                "Nguyễn Văn A",                  // name
                "0912345678",                    // phone
                "123 Lê Lợi",                    // address
                "Phường Bến Nghé",              // wardName
                "Quận 1",                        // districtName
                "TP. Hồ Chí Minh",              // provinceName
                "ORDER123456"                   // orderCode
            );
            return Ok(order);
        }
    }
}