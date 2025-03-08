using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using back.DTOs.Auth;
using back.DTOs.User;
using back.models;
using back.services;
using BackEnd.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back.controllers
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
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RegisterResponse))]

        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.Register(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("refreshToken")]
        public ActionResult<AuthenticateResponse> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = _authService.RefreshToken(request.refreshToken!);
                return Ok(new AuthenticateResponse
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }
        [HttpGet("get/user")]
        [Authorize]
        public async Task<ActionResult> GetUser([FromQuery] string user_id)
        {
            var user = await _authService.GetUser(user_id);
            return Ok(new {userInfor = user });
        }
        [HttpGet("get/all-user")]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public  async Task<ActionResult> GetAllUser([FromQuery] int pageSize)
        {
            var user = await  _authService.GetAllUser(pageSize);
            return Ok(new { users = user });
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
            try
            {
                await _authService.SendOTP(data);
                return Ok(new { message = "gửi otp thanh công vui lòng check email" });
            }
            catch (System.Exception ex)
            {

                return BadRequest(new { message = ex.Message });

            }
        }
        [HttpPost("update-info")]
        public async Task<ActionResult> UpdateInfo([FromBody] UpdateInfoRequest data)
        {
            try
            {
                await _authService.UpdateInfor(data);
                return Ok(new { message = "cap nhat thanh cong" });
            }
            catch (System.Exception ex)
            {

                return BadRequest(new { message = ex.Message });

            }
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest data)
        {
            try
            {
                await _authService.ResetPassword(data);
                return Ok(new { message = "reset success" });
            }
            catch (System.Exception ex)
            {

                return BadRequest(new { message = ex.Message });

            }
        }
        [HttpPost("lock")]
        [Authorize(Roles = nameof(ROLE.Admin))]
        public async Task<ActionResult> LockOrUnlock([FromBody] LockOrUnlockRequest data)
        {
            var result = await _authService.LockOrUnlock(data);
            return Ok(new { message = result ? "khóa tài khoản thành công" : "mở khóa tài khoản thành công" });
        }
    }
}