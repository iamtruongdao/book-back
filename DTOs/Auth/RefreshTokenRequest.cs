using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Auth
{
    public class RefreshTokenRequest
    {
        [Required]
        public string? refreshToken { get; set; }
    }
}