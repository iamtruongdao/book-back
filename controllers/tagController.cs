using Microsoft.AspNetCore.Mvc;
using BackEnd.models;
using BackEnd.services.Tag;
using BackEnd.DTOs.Tag;
using Microsoft.AspNetCore.Authorization;


namespace BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(ROLE.Admin))]
public class tagController : ControllerBase
{
    private readonly ITagService _tagService;

    public tagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
    {
        var tags = await _tagService.GetAllAsync(pageSize, pageNumber);
        return Ok(new { Code = 0, message = "ok", data = tags });
    }


    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(string id)
    {
        var tag = await _tagService.GetByIdAsync(id);
        return tag == null ? NotFound() : Ok(tag);
    }

    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug, [FromQuery] int pageSize, int pageNumber)
    {
        Console.WriteLine(pageNumber.ToString(), pageSize);
        var result = await _tagService.GetBySlugAsync(slug, pageSize, pageNumber);
        return Ok(new { Code = 0, message = "ok", data = result });
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateTagDto tag)
    {
        var result  =  await _tagService.CreateAsync(tag);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new {Code = 0,Message = "ok", data = result });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTagDto tag)
    {
        await _tagService.UpdateAsync(tag);
        return Ok(new {Code = 0,Message = "ok" });
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
     await _tagService.DeleteAsync(id);
        return Ok(new {Code = 0,Message = "ok"});
    }
}
