using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Discounts;
using BackEnd.services.Discounts;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class discountController : ControllerBase
    {

        private readonly IDiscountService _discountService;

        public discountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDiscount([FromBody] CreateDiscountDto discount)
        {
            var result = await _discountService.CreateDiscount(discount);
            return Ok(new { Code = 0, Message = "Discount created successfully", Data = result });
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllDiscounts(int pageSize, int pageNumber)
        {
            var result = await _discountService.GetAllDiscounts(pageSize, pageNumber);
            return Ok(new { Code = 0, Message = "ok", Data = result });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDiscount(string id)
        {
            var result = await _discountService.DeleteDiscount(id);
            return Ok(new { Code = 0, Message = "ok", Data = result });
        }
        [HttpPost("get-amount")]
        public async Task<IActionResult> GetAmount([FromBody] GetAmountRequest request)
        {
            var result = await _discountService.GetAmount(request);
            return Ok(new { Code = 0, Message = "ok", Data = result });
        }
        [HttpPut("update-discount")]
        public async Task<IActionResult> UpdateDiscount([FromBody] UpdateDiscountDto discount)
        {
            var result = await _discountService.UpdateDiscount(discount);
            return Ok(new { Code = 0, Message = "ok", Data = result });
        }

        
    }
}