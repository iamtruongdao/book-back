using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackEnd.services.UserDiscounts;
using BackEnd.DTOs.UserDiscounts;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/user-voucher")]
    [Authorize]
    public class userVoucherController : ControllerBase
    {
        private readonly IUserDiscountService _userDiscountService;

        public userVoucherController(IUserDiscountService userDiscountService)
        {
            _userDiscountService = userDiscountService;
        }

        [HttpGet("get-user-voucher")]
        public async Task<ActionResult> GetUserVoucher()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
            var res = await _userDiscountService.GetUserDiscount(UserId);
            return Ok(new { Code = 0, message = "ok", data = res });
        }
        [HttpPost("create-user-voucher")]
        public async Task<ActionResult> CreateUserVoucher([FromBody] SaveDiscountDto data)
        {
            var res = await _userDiscountService.CreateUserDiscount(data);
            return Ok(new { Code = 0, message = "ok", data = res });
        }

    }

   
}