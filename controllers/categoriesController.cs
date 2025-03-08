using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using back.DTOs.Categories;
using back.models;
using back.services;
using back.Viewmodel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class categoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;

        public categoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }    
        
        [HttpPost]
        public async Task<ActionResult> AddCat([FromBody] CreateCatDTO cat)
        {
            var createdCat = await _categoriesService.AddCat(cat);
            return createdCat is null ? BadRequest("Cat is invalid") : CreatedAtAction("GetCat", new {slug = createdCat.Slug}, createdCat);
        }
        [HttpGet]
        [ProducesResponseType(200)]
      
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCat()
        {
            var cats = await _categoriesService.GetAllCat();
            return Ok(new { EC = 0, categories = cats });
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCat(string id)
        {
            var cats = await _categoriesService.DeleteCat(id);
            return Ok(new {EC = 0, categories = cats});
        }
        [HttpPut]
        public async Task<ActionResult> UpdateCat([FromBody] UpdateCatDTO cat)
        {
            if(cat == null) return BadRequest("cat is invalid");
            var cats = await _categoriesService.UpdateCat(cat);
            return Ok(new {EC = 0, categories = cats});
        }
        [HttpGet("{slug}")]
        public async Task<ActionResult<Category>> GetCat( string slug)
        {
            var cat = await _categoriesService.GetCat(slug);
            return cat is null ? NotFound("cat not found") : Ok(new {EC = 0,msg = "ok", cat = cat});
        }
        
       
        [HttpGet("paginate")]
        public async Task<ActionResult<PaginatedList<Category>>> GetAllFilter([FromQuery] string? sortOrder, string? currentFilter, string? searchString, int pageNumber, int? pageSize)
        {
            var cats = await _categoriesService.GetAllFilter(sortOrder ?? "", currentFilter ?? "", searchString ?? "", pageNumber, pageSize ?? 10);
            return Ok(new {EC = 0, categories= cats});
        }
    }
}