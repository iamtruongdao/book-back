using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using back.DTOs.Auth;
using back.DTOs.User;
using back.models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace back.services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
       
        public AuthService( UserManager<User> userManager, IMapper mapper,ITokenService tokenService)
        {
            _tokenService = tokenService;
            _mapper = mapper;   
            _userManager = userManager;
        }
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var userExist = await _userManager.FindByEmailAsync(request.Email!);
            if(userExist is null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid Email/Password"
                };
            }
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Sub, userExist.Id.ToString()),
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
                token = new AuthenticateResponse{AccessToken = accessToken,RefreshToken = refreshToken},
                Success = true,
                Email = userExist.Email,
                UserId = userExist.Id.ToString(),
                Message = "Login success"
            };
        }

        
        public AuthenticateResponse RefreshToken(string token)
        {        
            var claimsPrincipal = _tokenService.GetPrincipalFromExpiredToken(token);
            var newAccessToken = _tokenService.GenerateAccessToken(claimsPrincipal.Claims);
            var newRefreshToken = _tokenService.GenerateAccessToken(claimsPrincipal.Claims);
            return new AuthenticateResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
                
        }
            
        

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            var userExist = await _userManager.FindByEmailAsync(request.Email!);
            if(userExist != null)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Email already exist"
                };
            }
            userExist = _mapper.Map<User>(request);
            var createdResult = await _userManager.CreateAsync( userExist,request.Password!);
            if(!createdResult.Succeeded) return  new RegisterResponse{
                Success = false,
                Message = $"Create User failed {createdResult.Errors.First().Description}"
            };
            var addRoleUserResult = await _userManager.AddToRoleAsync(userExist, ROLE.User.ToString());
            if(!addRoleUserResult.Succeeded) return  new RegisterResponse{
                Success = false,
                Message = $"Create User success but could not add role User {createdResult.Errors.First().Description}"
            };
            return new RegisterResponse
            {
                Success = true,
                Message = "Create User success"
            };
        }
    }
}