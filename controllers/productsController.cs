using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Product;
using BackEnd.models;
using BackEnd.services;
using BackEnd.Viewmodel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class productsController : ControllerBase
    {
        private readonly IProductService _productService;


        public productsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public async Task<ActionResult> AddProduct([FromBody] CreateProductDTO product)
        {
            var createdProduct = await _productService.AddProduct(product);
            return createdProduct is null ? BadRequest("product is invalid") : CreatedAtAction("GetProduct", new { slug = createdProduct.Slug }, new { Code = 0, message = "ok", data = createdProduct });
        }
        [HttpPost("all")]
        public async Task<ActionResult> AddProductMany([FromBody] List<CreateProductDTO> product)
        {
            try
            {
                var createdProduct = await _productService.AddProductMany(product);
                return createdProduct is null ? BadRequest("product is invalid") : Ok(new { products = createdProduct });
            }
            catch (System.Exception ex)
            {

                Console.WriteLine(ex);
                return BadRequest("product is invalid");
            }

        }
        [HttpGet]
        // [Authorize]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProduct()
        {
            var products = await _productService.GetAllProduct();
            return Ok(new { Code = 0, products = products });
        }
        [HttpGet("slide")]

        public async Task<ActionResult<IEnumerable<Product>>> GetSliderProduct([FromQuery] int limit)
        {
            var products = await _productService.GetSliderProduct(limit);
            if (products.Count == 0) return BadRequest("not found product");
            return Ok(new { Code = 0, products = products });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            var products = await _productService.DeleteProduct(id);
            return Ok(new { Code = 0, message = "ok", data = products });
        }
        [HttpPut]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductDTO product)
        {
            if (product == null) return BadRequest("product is invalid");
            var products = await _productService.UpdateProduct(product);
            return Ok(new { Code = 0, products = products });
        }
        [HttpGet("{slug}")]
        public async Task<ActionResult<Product>> GetProduct(string slug)
        {
            var product = await _productService.GetProduct(slug);
            return product is null ? NotFound("product not found") : Ok(new { Code = 0, message = "ok", data = product });
        }
        [HttpGet("get-by-author/{author}")]
        public async Task<ActionResult<Product>> GetProductByAuthor(string author)
        {
            var product = await _productService.GetProductByAuthor(author);
            return Ok(new { Code = 0, message = "ok", data = product });
        }

        [HttpGet("paginate")]
        // [Authorize(Roles = nameof(ROLE.User))]

        public async Task<ActionResult<PaginatedList<Product>>> GetAllFilter([FromQuery] string? sortOrder, string? currentFilter, string? searchString, string? cate, int pageNumber, int? pageSize, decimal minPrice, decimal maxPrice)
        {

            var products = await _productService.GetAllFilter(sortOrder ?? "", currentFilter ?? "", searchString ?? "", cate ?? "", pageNumber, pageSize ?? 10, minPrice, maxPrice);
            return Ok(new { Code = 0, message = "ok", data = products });
        }
        [HttpGet("wait-publish")]
        public async Task<ActionResult<PaginatedList<Product>>> GetProductWaitPublish([FromQuery] int pageSize = 10, int pageNumber = 1)
        {
            var products = await _productService.GetProductWaitPublish(pageSize, pageNumber);
            return Ok(new { Code = 0, message = "ok", data = products });
        }
        [HttpGet("top-product")]
        public async Task<ActionResult<PaginatedList<Product>>> GetTopProduct([FromQuery] int year, int pageSize = 5, int pageNumber = 1)
        {
            var products = await _productService.GetTopProduct(year, pageSize, pageNumber);
            return Ok(new { Code = 0, message = "ok", data = products });
        }
    }
}