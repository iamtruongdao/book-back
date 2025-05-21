using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Auth;

namespace BackEnd.DTOs.User
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public AuthenticateResponse? token { get; set; }

        public UserResponse? UserAccount { get; set; }
        public List<string>? Roles { get; set; }
    }
}