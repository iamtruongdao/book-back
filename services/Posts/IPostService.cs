using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Viewmodel;
using BackEnd.DTOs.Posts;

using BackEnd.models;
namespace BackEnd.services.Posts
{
    public interface IPostService
    {
        
        Task<PaginatedList<PostDto>> GetAllPosts(int pageNumber, int pageSize);
        Task<Post> GetPostById(string id);
        Task<PostDto> GetPostBySLug(string slug);
        Task<Post> CreatePostAsync(CreatePostDto post);
        Task<List<Post>> CreatePostManyAsync(List<CreatePostDto> post);
        Task<List<Post>> GetByTag(string tag);
        Task UpdatePostAsync(UpdatePostDto post);
        Task DeletePostAsync(string id);
    }
}