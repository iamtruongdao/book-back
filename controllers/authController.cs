using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using BackEnd.DTOs.Auth;
using BackEnd.DTOs.User;
using BackEnd.models;
using BackEnd.services;

using BackEnd.Exceptions;
using Google.Rpc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BackEnd.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class authController : ControllerBase
    {
        private readonly IAuthService _authService;

        public authController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Login(request);
            Response.Cookies.Append("act", result.token!.AccessToken!,new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None

            });
            Response.Cookies.Append("rft", result.token!.RefreshToken!,new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
            return Ok(new {Code = 0, message = "login success", data = result });
        }
        [HttpPost("logout")]
        public ActionResult Logout()
        {
             var options = new CookieOptions
            {
                Expires = DateTimeOffset.UnixEpoch,
                SameSite = SameSiteMode.None,
                Secure = true,
            };
            Response.Cookies.Delete("act",options);
            Response.Cookies.Delete("rft",options);
            return Ok(new {Code = 0, message = "logout success" });
        }
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RegisterResponse))]

        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.Register(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("refreshToken")]
        public ActionResult<AuthenticateResponse> RefreshToken()
        {
            Request.Cookies.TryGetValue("rft", out string? rft);
            var result = _authService.RefreshToken(rft);
            Response.Cookies.Append("act", result.AccessToken!,new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None

            });
            Response.Cookies.Append("rft", result.RefreshToken!,new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
             return Ok(new AuthenticateResponse
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken
            });
        }
        [HttpGet("get/user")]
        [Authorize]
        public async Task<ActionResult> GetUser()
        {
            var user_id = Request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _authService.GetUser(user_id!);
            return Ok(new {Code = 0,message = "ok", data = user });
        }
        [HttpGet("get/all-user")]
        // [Authorize(Roles = nameof(ROLE.Admin))]
        public  async Task<ActionResult> GetAllUser([FromQuery] int pageSize,int pageNumber)
        {
            var user = await  _authService.GetAllUser(pageSize,pageNumber);
            return Ok(new {Code = 0,mesage ="ok",  data = user });
        }
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest data)
        {
            try
            {
                var result = await _authService.ChangePassword(data);
                if (result)
                {
                    return Ok(new { message = "doi mk thanh cong" });
                }
                return BadRequest(new { message = "doi mk k thanh cong" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("send-otp")]
        public async Task<ActionResult> SendOtp([FromBody] SendOtpRequest data)
        {
            
                await _authService.SendOTP(data);
                return Ok(new {Code = 0, Message = "gửi otp thanh công vui lòng check email" });
            
        }
        [HttpPost("verify-otp")]
         [Authorize]
        public async Task<ActionResult> VerifyOtp([FromBody] SendOtpRequest data)
        {

            await _authService.VerifyOtp(data);
            return Ok(new { Code = 0, Message = "verify thành công" });

        }
        [HttpPost("update-info")]
        [Authorize]
        public async Task<ActionResult> UpdateInfo([FromBody] UpdateInfoRequest data)
        {
            await _authService.UpdateInfor(data);
            return Ok(new { Code = 0, message = "cap nhat thanh cong" });
        }
        [HttpPost("reset-password")]
    
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest data)
        {
            await _authService.ResetPassword(data);
            return Ok(new { Code = 0, message = "reset success" });
        }
        [HttpPost("lock")]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public async Task<ActionResult> LockOrUnlock([FromBody] LockOrUnlockRequest data)
        {
            var result = await _authService.LockOrUnlock(data);
            return Ok(new {Code = 0,  message = result ? "khóa tài khoản thành công" : "mở khóa tài khoản thành công" });
        }
    }
}