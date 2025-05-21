using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.User
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}