using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using back.DTOs.Cart;
using back.models;
using back.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace back.controllers
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
                if (cart == null) return BadRequest();
                return Ok(cart);
            
        }
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteCart([FromBody] DeleteItemDTO item)
        {
            var cart = await _cartService.DeleteCart(item);
            if (cart == null) return BadRequest();
            return Ok(cart);
        }
        [HttpPost("update-quantity")]
        public async Task<IActionResult> IncOrDecProductQuantity([FromBody] IncOrDecProductQuantityDTO item)
        {
            var cart = await _cartService.IncOrDecProductQuantity(item);
            if (cart == null) return BadRequest();
            return Ok(cart);
        }
        [HttpGet("user")]
        [Authorize(Roles = nameof(ROLE.User))]
        
        public IActionResult GetCart([FromQuery] string user_id)
        {
            var cart = _cartService.GetCart(user_id);
            if (cart == null) return BadRequest();
            return Ok(cart);
        }
    }
}