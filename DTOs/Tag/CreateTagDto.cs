using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slugify;

namespace BackEnd.DTOs.Tag
{
    public class CreateTagDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Slug => new SlugHelper().GenerateSlug(Name);
    }
}