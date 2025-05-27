using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? Otp { get; set; }
    }
}