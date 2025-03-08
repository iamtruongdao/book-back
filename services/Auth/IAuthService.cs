using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Auth;

using back.DTOs.User;
using BackEnd.DTOs.Auth;

namespace back.services
{
    public interface IAuthService
    {
        public Task<LoginResponse> Login(LoginRequest request);
        public Task<RegisterResponse> Register(RegisterRequest request);
        public AuthenticateResponse RefreshToken(string token);
        public Task<UserResponse> GetUser(string user_id);
        public Task<bool> ChangePassword(ChangePasswordRequest data);
        public Task<bool> ResetPassword(ResetPasswordRequest data);
        public Task SendOTP(SendOtpRequest data);
        public Task UpdateInfor(UpdateInfoRequest data);
        public Task<bool> LockOrUnlock(LockOrUnlockRequest data);
        public Task<List<UserResponse>> GetAllUser(int limit);
       
    }
}