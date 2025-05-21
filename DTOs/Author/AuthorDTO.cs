using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace BackEnd.DTOs.Author
{
    public class AuthorDTO
    {
        public ObjectId _id { get; set; }
        public string? AuthorName { get; set; } 
        public string? AuthorDescription { get; set; } 
        public string? Avatar { get; set; } 
        public string? Slug { get; set; } 
    }
    public class CreateAuthorDTO
    {
        [Required(ErrorMessage = "Author name is required")]
        [StringLength(100)]
        public string? AuthorName { get; set; } 
        public string? AuthorDescription { get; set; } 
        public string? Avatar { get; set; } 
        public string? Slug { get; set; }

    }
    public class UpdateAuthorDTO
    {
        [Required(ErrorMessage = "Author id is required")]
        public string Id { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorDescription { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}