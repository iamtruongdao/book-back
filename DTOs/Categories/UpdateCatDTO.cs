using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back.DTOs.Categories
{
    public class UpdateCatDTO
    {
        public string? Id { get; set; }
        public string? Name { get; set; } 
        public string? Slug {get;set;}
    }
}