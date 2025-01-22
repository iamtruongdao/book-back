using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Auth;

namespace back.DTOs.User
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public AuthenticateResponse? token { get; set; }
        public string? Email { get; set; }
        public string? UserId { get; set; }
    }
}