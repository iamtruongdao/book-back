using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using back.DTOs.Auth;
using back.DTOs.User;
using back.services;
using Microsoft.AspNetCore.Mvc;

namespace back.controllers
{
    [ApiController]
    [Route("api/authenticate")]
    public class authController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public authController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        [ProducesResponseType((int) HttpStatusCode.OK,Type = typeof(LoginResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Login(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.Register(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPost("refreshToken")]
        public  ActionResult<AuthenticateResponse> RefreshToken([FromBody] RefreshTokenRequest request) {
            try
            {
                var result =  _authService.RefreshToken(request.refreshToken!);
                return new AuthenticateResponse {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken
                };
            }
            catch (Exception ex)
            {

                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }
    }
}