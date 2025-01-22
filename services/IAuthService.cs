using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Auth;
using back.DTOs.User;

namespace back.services
{
    public interface IAuthService
    {
        public Task<LoginResponse> Login(LoginRequest request);
        public Task<RegisterResponse> Register(RegisterRequest request);
        public AuthenticateResponse RefreshToken(string token);
    }
}