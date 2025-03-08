using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Auth
{
    public class LockOrUnlockRequest
    {
        [Required]
        public bool IsLock { get; set; }
        [Required]
        public string? UserId { get; set; }
    }
}