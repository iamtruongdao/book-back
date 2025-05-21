using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Auth
{
    public class SendOtpRequest
    {
        [Required]
        public string? Email { get; set; }
        public string? Otp { get; set; }
    }
}