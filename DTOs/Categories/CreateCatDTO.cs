using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slugify;

namespace BackEnd.DTOs.Categories
{
    public class CreateCatDTO
    {
        public string? Name { get; set; }
        public string SLug => new SlugHelper().GenerateSlug(Name); 
    }
}