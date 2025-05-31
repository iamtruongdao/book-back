
using System.Security.Claims;
using BackEnd.DTOs.Cart;
using BackEnd.models;
using BackEnd.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BackEnd.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    
    public class cartController : ControllerBase
    {
        private readonly ICartServices _cartService;
        public cartController(ICartServices cartService)
        {
            _cartService = cartService;
        }
        [HttpPost("add")]
        public async Task<ActionResult> AddProductToCart([FromBody] AddProductToCartDTO product)
        {
            
            var cart = await _cartService.AddProductToCart(product);
            return Ok(new {Code = 0,Message = "ok",Data = cart});
            
        }
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteCart([FromBody] DeleteItemDTO item)
        {
            var cart = await _cartService.DeleteCart(item);
            return Ok(new {Code = 0,Message = "ok",Data = cart});

        }
        [HttpPost("update-quantity")]
        public async Task<IActionResult> IncOrDecProductQuantity([FromBody] IncOrDecProductQuantityDTO item)
        {
            var cart = await _cartService.IncOrDecProductQuantity(item);
            return Ok(new {Code = 0,Message = "ok",Data = cart});
        }
        [HttpGet("user")]
        [Authorize]
        
        public IActionResult GetCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cart = _cartService.GetCart(userId!);
            return Ok(new {Code = 0,Message = "ok",Data = cart});
        }
    }
}