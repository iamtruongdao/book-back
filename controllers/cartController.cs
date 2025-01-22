using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Cart;
using back.services;
using Microsoft.AspNetCore.Mvc;

namespace back.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class cartController : ControllerBase
    {
        private readonly ICartServices _cartService;
        public cartController(ICartServices cartService) {
            _cartService = cartService;
        }
        [HttpPost]
        public async Task<ActionResult> AddProductToCart([FromBody] AddProductToCartDTO product) {
            var cart = await _cartService.AddProductToCart(product);
            if (cart == null) return BadRequest();
            return Ok(cart);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteCart([FromBody] DeleteItemDTO item) {
            var cart = await _cartService.DeleteCart(item);
            if (cart == null) return BadRequest();
            return Ok(cart);
        }
        [HttpPost("update-quantity")]
        public async Task<IActionResult> IncOrDecProductQuantity([FromBody] IncOrDecProductQuantityDTO item) {
            var cart = await _cartService.IncOrDecProductQuantity(item);
            if (cart == null) return BadRequest();
            return Ok(cart);
        }
    }
}