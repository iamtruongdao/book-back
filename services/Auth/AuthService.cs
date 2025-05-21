using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

using AutoMapper;
using BackEnd.DTOs.Auth;

using BackEnd.DTOs.User;
using BackEnd.models;
using BackEnd.services.Email;
using BackEnd.Viewmodel;

using BackEnd.Exceptions;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace BackEnd.services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<User> userManager, IMapper mapper, ITokenService tokenService, IEmailService emailService)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
        }



        public async Task<bool> ChangePassword(ChangePasswordRequest data)
        {
            var user = await _userManager.FindByIdAsync(data.UserId!);
            if (user is null) throw new Exception("user is not Exist");
            var result = await _userManager.ChangePasswordAsync(user, data.Password!, data.NewPassword!);
            return result.Succeeded;
        }

        public async Task<PaginatedList<UserResponse>> GetAllUser(int limit,int pageNumber)
        {
            var list = _userManager.Users.AsQueryable().Take(limit).Skip((pageNumber-1)*limit).ToList();
            var total = _userManager.Users.Count();
            foreach (var item in list)
            {
                item.Roles = (await _userManager.GetRolesAsync(item)).ToList();

            }
            var userRes = _mapper.Map<List<UserResponse>>(list);

            return new PaginatedList<UserResponse>(userRes, total, pageNumber, limit);
        }

        public async Task<UserResponse> GetUser(string user_id)
        {
            var user = await _userManager.FindByIdAsync(user_id);
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<bool> LockOrUnlock(LockOrUnlockRequest data)
        {
            var user = await _userManager.FindByIdAsync(data.UserId!);
            if (user is null) throw new Exception("user not found");
            if (data.IsLock)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                return true;
            }

            await _userManager.SetLockoutEndDateAsync(user, null);
            return false;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var userExist = await _userManager.FindByEmailAsync(request.Email!);
            if (userExist is null)
            {
                throw new NotFoundException("Invalid Email/Password");
                
            }
            if (await _userManager.IsLockedOutAsync(userExist))
            {
                throw new NotFoundException("Tài Khoản của bạn đã bị khóa");
            }
            var checkPassword = await _userManager.CheckPasswordAsync(userExist, request.Password!);
            if (!checkPassword) 
            {
             throw new NotFoundException("Invalid Email/Password");

            };  
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Sub, userExist.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userExist.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userExist.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name,userExist.FullName!)
            };
            var roles = await _userManager.GetRolesAsync(userExist);
            var rolesClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
            claims.AddRange(rolesClaims);
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken(claims);

            return new LoginResponse
            {
                token = new AuthenticateResponse { AccessToken = accessToken, RefreshToken = refreshToken },
                Success = true,
                UserAccount = _mapper.Map<UserResponse>(userExist),
                Message = "Login success",
                Roles = roles.ToList()
            };
        }
        

        public AuthenticateResponse RefreshToken(string? token)
        {
            if (token is null) throw new UnAuthorizeException("please login");
            var claimsPrincipal = _tokenService.GetPrincipalFromExpiredToken(token);
            var newAccessToken = _tokenService.GenerateAccessToken(claimsPrincipal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken(claimsPrincipal.Claims);
            return new AuthenticateResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

        }
        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            var userExist = await _userManager.FindByEmailAsync(request.Email!);
            if (userExist != null)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Email already exist"
                };
            }
            userExist = _mapper.Map<User>(request);
            var createdResult = await _userManager.CreateAsync(userExist, request.Password!);
            if (!createdResult.Succeeded) return new RegisterResponse
            {
                Success = false,
                Message = $"Create User failed {createdResult.Errors.First().Description}"
            };
            var addRoleUserResult = await _userManager.AddToRoleAsync(userExist, ROLE.User.ToString());
            if (!addRoleUserResult.Succeeded) return new RegisterResponse
            {
                Success = false,
                Message = $"Create User success but could not add role User {createdResult.Errors.First().Description}"
            };
            return new RegisterResponse
            {
                Success = true,
                Message = "Create User success"
            };
        }

        public async Task<bool> ResetPassword(ResetPasswordRequest data)
        {
            var user = await _userManager.FindByEmailAsync(data.Email!);
            if (user is null) throw new NotFoundException("user not found");
         
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, data.Password!);
            return true;
        }

        public async Task SendOTP(SendOtpRequest data)
        {
            var user = await _userManager.FindByEmailAsync(data.Email!);
            if (user is null) throw new NotFoundException("error user not found");
            int otp = RandomNumberGenerator.GetInt32(100000, 999999);
            user.Otp = otp.ToString();
            user.OtpExprire = DateTime.Now.AddMinutes(5);
            await _userManager.UpdateAsync(user);
            await _emailService.Sendmail(data.Email!, "Reset password", $"Otp của bạn là {otp}");
        }
        public async Task UpdateInfor(UpdateInfoRequest data)
        {
            var user = await _userManager.FindByEmailAsync(data.Email!);
            if (user is null) throw new NotFoundException("error user not found");
            
            user.FullName = data.FullName;
            user.PhoneNumber = data.PhoneNumber;
            user.Address = data.Address;
            await _userManager.UpdateAsync(user);
            
        }

        public async Task VerifyOtp(SendOtpRequest data)
        {
            var user = await _userManager.FindByEmailAsync(data.Email!);
            if (user is null) throw new NotFoundException("error user not found");
            if (user.Otp != data.Otp)
            {
                throw new NotFoundException("otp invalid or expire time");
            }
          
        }
    }
}