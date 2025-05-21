using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.User
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string? Email { get; set; }
        public string? FullName { get; set; }
        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required, DataType(DataType.Password),Compare(nameof(Password),ErrorMessage = "Password and Confirm Password do not match")]
        public string? ConfirmPassword { get; set; }
    }
}