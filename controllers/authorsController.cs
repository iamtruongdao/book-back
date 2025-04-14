using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Author;

using back.models;
using back.services;
using back.Viewmodel;
using Microsoft.AspNetCore.Mvc;

namespace back.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class authorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public authorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }    
        
        [HttpPost]
        public async Task<ActionResult> AddAuthor([FromBody] CreateAuthorDTO author)
        {
            var createdAuthor = await _authorService.AddAuthor(author);
            return createdAuthor is null ? BadRequest("Author is invalid") : CreatedAtAction("GetAuthor", new {slug = createdAuthor.Slug}, createdAuthor);
        }
         [HttpPost("all")]
        public async Task<ActionResult> AddAuthorMany([FromBody] List<CreateAuthorDTO> author)
        {
            var createdAuthor = await _authorService.AddAuthorMany(author);
            return Ok(new {Code = 0, authors = createdAuthor});
        }
        [HttpGet]
        [ProducesResponseType (200)]
        public async Task<ActionResult<IEnumerable<Author>>> GetAllAuthor()
        {
            var authors = await _authorService.GetAllAuthor();
            return Ok(new {Code = 0,message = "ok", authors = authors});
        }
        [HttpGet("string")]
        [ProducesResponseType (200)]
        public async Task<ActionResult> GetStrings()
        {
            var authors = await _authorService.GetStrings();
            return Ok(new {Code = 0,message = "ok", authors = authors});
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuthor(string id)
        {
            var authors = await _authorService.DeleteAuthor(id);
            return Ok(new {Code = 0,message = "ok", data = authors});
        }
        [HttpPut]
        public async Task<ActionResult> UpdateAuthor([FromBody] UpdateAuthorDTO author)
        {
            if(author == null) return BadRequest("author is invalid");
            var authors = await _authorService.UpdateAuthor(author);
            return Ok(new {Code = 0,message="update ok", data = authors});
        }
        [HttpGet("{slug}")]
        public async Task<ActionResult<Author>> GetAuthor(string slug)
        {
            var author = await _authorService.GetAuthor(slug);
            return Ok(new {Code = 0,message = "ok", data = author});
        }
        [HttpGet("paginate")]
        public async Task<ActionResult<PaginatedList<Author>>> GetAllFilter([FromQuery] string? sortOrder, string? currentFilter, string? searchString, int pageNumber, int? pageSize)
        {
            var authors = await _authorService.GetAllFilter(sortOrder ?? "", currentFilter ?? "", searchString ?? "", pageNumber, pageSize ?? 10);
            return Ok(new {Code = 0,message = "ok",data = authors});
        }
    }
}