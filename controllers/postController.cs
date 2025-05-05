using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Posts;
using BackEnd.services.Posts;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class postController : ControllerBase
    {

        private readonly IPostService _postService;
        public postController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpGet("paginate")]
        public async Task<IActionResult> GetAllPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var posts = await _postService.GetAllPosts(pageNumber,pageSize);
            return Ok(new { Code = 0, Message = "ok", Data = posts });
        }
       

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(string id)
        {
            var post = await _postService.GetPostById(id);
            return Ok(new { Code = 0, Message = "ok", Data = post });
        }
        [HttpGet("get-by-slug/{slug}")]
        public async Task<IActionResult> GetPostBySlug(string slug)
        {

            var post = await _postService.GetPostBySLug(slug);  
            return Ok(new { Code = 0, Message = "ok", Data = post });
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            await _postService.DeletePostAsync(id);
            return Ok(new { Code = 0, Message = "ok" });
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePostDto post)
        {
            await _postService.UpdatePostAsync(post);
            return Ok(new { Code = 0, Message = "ok" });
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto post)
        {
            await _postService.CreatePostAsync(post);
            return Ok(new { Code = 0, Message = "ok" });
        }

        [HttpPost("create-many")]
        public async Task<IActionResult> CreatePostMany([FromBody] List<CreatePostDto> post)
        {
            await _postService.CreatePostManyAsync(post);
            return Ok(new { Code = 0, Message = "ok" });
        }
        [HttpGet("get-by-tag/{tag}")]
        public async Task<IActionResult> GetByTag(string tag)
        {
            var posts = await _postService.GetByTag(tag);
            return Ok(new { Code = 0, Message = "ok", Data = posts });
        }
    }
}