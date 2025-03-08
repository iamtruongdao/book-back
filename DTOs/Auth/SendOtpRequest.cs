using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace back.DTOs.Auth
{
    public class SendOtpRequest
    {
        [Required]
        
        public string? Email { get; set; }
       
        
    }
}