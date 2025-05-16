using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Posts
{
    public class UpdatePostDto:CreatePostDto
    {
        public string? Id { get; set; }
    }
}