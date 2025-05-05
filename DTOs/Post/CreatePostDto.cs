using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Posts
{
    public class CreatePostDto
    {
        public string? Thumbnail { get; set; }
        public string? Title { get; set; }
        public string Slug => new Slugify.SlugHelper().GenerateSlug(Title);
        public string? TagId { get; set; }
        public string? Content { get; set; } 
        public DateTime CreatedAt { get; set; }
       
    }
}