using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Product;
using back.models;
using back.services;
using back.Viewmodel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class productsController : ControllerBase
    {
        private readonly IProductService _productService;

     
        public  productsController(IProductService productService)
        {
            _productService = productService;
         
        }
        
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] CreateProductDTO product)
        {
            var createdProduct = await _productService.AddProduct(product);
            return createdProduct is null ? BadRequest("product is invalid") : CreatedAtAction("GetProduct", new {slug = createdProduct.Slug}, createdProduct);
        }
        [HttpGet]
    
        // [Authorize]
        [ProducesResponseType (200)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProduct()
        {
            var products = await _productService.GetAllProduct();
            return Ok(new {EC = 0, products = products});
        }
        [HttpDelete("{id}:length(24)")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            var products = await _productService.DeleteProduct(id);
            return Ok(new {EC = 0, products = products});
        }
        [HttpPut]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductDTO product)
        {
            if(product == null) return BadRequest("product is invalid");
            var products = await _productService.UpdateProduct(product);
            return Ok(new {EC = 0, products = products});
        }
        [HttpGet("{slug}")]
        public async Task<ActionResult<Product>> GetProduct(string slug)
        {
            var product = await _productService.GetProduct(slug);
            return product is null ? NotFound("product not found") : Ok(new {EC = 0, product = product});
        }
        
       
        [HttpGet("paginate")]
        [Authorize(Roles = nameof(ROLE.User))]
        
        public async Task<ActionResult<PaginatedList<Product>>> GetAllFilter([FromQuery] string? sortOrder, string? currentFilter, string? searchString, int pageNumber, int? pageSize)
        {
            
            var context = HttpContext.User;
            var products = await _productService.GetAllFilter(sortOrder ?? "", currentFilter ?? "", searchString ?? "", pageNumber, pageSize ?? 10);
            return Ok(new {EC = 0, products = products});
        }
    }
}