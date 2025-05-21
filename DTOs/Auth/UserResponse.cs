using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Auth
{
    public class UserResponse
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string?  Address { get; set; }
        public string?  PhoneNumber { get; set; }
        public string? Email { get; set; } 
        public bool IsLocked { get; set; }
        public List<string>? Roles { get; set; } 
      
    }
}