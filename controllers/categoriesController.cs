using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BackEnd.DTOs.Categories;
using BackEnd.models;
using BackEnd.services;
using BackEnd.Viewmodel;
using BackEnd.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BackEnd.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(ROLE.Admin))]
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
            return createdCat is null ? BadRequest("Cat is invalid") : CreatedAtAction("GetCat", new { slug = createdCat.Slug }, new { Code = 0, message = "ok", data = createdCat });
        }
        [HttpGet]
        [ProducesResponseType(200)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCat()
        {
            var cats = await _categoriesService.GetAllCat();
            return Ok(new { Code = 0, message = "ok", data = cats });
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCat(string id)
        {
            var cats = await _categoriesService.DeleteCat(id);
            return Ok(new { Code = 0, data = cats });
        }
        [HttpPut]
        public async Task<ActionResult> UpdateCat([FromBody] UpdateCatDTO cat)
        {
            if (cat == null) return BadRequest("cat is invalid");
            var cats = await _categoriesService.UpdateCat(cat);
            return Ok(new { Code = 0, data = cats });
        }
        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetCat(string slug)
        {
            var cat = await _categoriesService.GetCat(slug);
            return cat is null ? NotFound("cat not found") : Ok(new { Code = 0, msg = "ok", data = cat });
        }


        [HttpGet("paginate")]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedList<Category>>> GetAllFilter([FromQuery] PaginateRequest request)
        {
            var cats = await _categoriesService.GetAllFilter(request.SortOrder ?? "", request.CurrentFilter ?? "", request.SearchString ?? "", request.PageNumber, request.PageSize);
            return Ok(new { Code = 0, message = "ok", data = cats });
        }
    }
}