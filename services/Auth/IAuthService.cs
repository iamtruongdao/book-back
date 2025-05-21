using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Auth;

using BackEnd.DTOs.User;
using BackEnd.Viewmodel;

using Microsoft.AspNetCore.Mvc;

namespace BackEnd.services
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest request);
        //  Task<ActionResult> LogOut();
        Task<RegisterResponse> Register(RegisterRequest request);
        AuthenticateResponse RefreshToken(string? token);
        Task<UserResponse> GetUser(string user_id);
        Task<bool> ChangePassword(ChangePasswordRequest data);
        Task<bool> ResetPassword(ResetPasswordRequest data);
        Task SendOTP(SendOtpRequest data);
        Task UpdateInfor(UpdateInfoRequest data);
        Task<bool> LockOrUnlock(LockOrUnlockRequest data);
        Task<PaginatedList<UserResponse>> GetAllUser(int limit,int pageNumber);
        Task VerifyOtp(SendOtpRequest otp);
       
    }
}