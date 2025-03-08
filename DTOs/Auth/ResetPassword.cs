using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back.DTOs.Auth
{
    public class ResetPasswordRequest
    {
        public string? Email { get; set; }
        public string? Otp { get; set; }
        public string? Password { get; set; }
    }
}