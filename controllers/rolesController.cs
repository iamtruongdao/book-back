using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Role;
using BackEnd.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class rolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public rolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest roleName)
        {
            var result = await _roleService.CreateRole(roleName);
            return result ? Ok(new { Message = "create success" }) : BadRequest(new
            {
                Message = "create failed"
            });
        }
    }
}