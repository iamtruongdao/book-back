using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back.DTOs.Auth
{
    public class AuthenticateResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}