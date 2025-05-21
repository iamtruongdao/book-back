using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.services;
using BackEnd.Viewmodel;
using BackEnd.DTOs.Posts;

using BackEnd.Exceptions;
using BackEnd.models;
using BackEnd.Repository;

namespace BackEnd.services.Posts
{       
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly ICloundinaryService _cloundinaryService;
        public PostService(ICloundinaryService cloundinaryService, IPostRepository postRepository, IMapper mapper)
        {
            _cloundinaryService = cloundinaryService;
            _postRepository = postRepository;
            _mapper = mapper;
        }
       
      
        public async Task<Post> CreatePostAsync(CreatePostDto post)
        {
            if (post.Thumbnail != null)
            {
                var uploadResult = await _cloundinaryService.uploadImage(post.Thumbnail);
                post.Thumbnail = uploadResult;
            }
            var postToCreate = _mapper.Map<Post>(post);
            var result = await _postRepository.CreatePostAsync(postToCreate);
            if (String.IsNullOrEmpty(result.Id)) throw new BadRequestException("Post not created");
            return result;
        }

        public Task<List<Post>> CreatePostManyAsync(List<CreatePostDto> post)
        {
            var postToCreate = _mapper.Map<List<Post>>(post);
            return _postRepository.CreatePostMany(postToCreate);
        }

        public Task DeletePostAsync(string id)
        {
            return _postRepository.DeletePostAsync(id);
        }

        public Task<PaginatedList<PostDto>> GetAllPosts(int pageNumber, int pageSize)
       
        {
            return _postRepository.GetAllPosts(pageNumber, pageSize);
        }

        public Task<List<Post>> GetByTag(string tag)
        {
            return _postRepository.FindByTags(tag);
        }

        public async Task<Post> GetPostById(string id)
        {
            var post = await _postRepository.FindById(id);
            if (post == null)  throw new NotFoundException("Post not found");
            return post;
        }

        public async Task<PostDto> GetPostBySLug(string slug)
        {
            var post = await _postRepository.FindBySLug(slug);
            if (post == null)  throw new NotFoundException("Post not found");
            return post;    
        }

        public async Task UpdatePostAsync(UpdatePostDto post)
        {
            Console.WriteLine(post.Id);
            var postToUpdate = _mapper.Map<Post>(post);
            var result = await _postRepository.UpdatePostAsync(postToUpdate);
            if(result.MatchedCount == 0) throw new BadRequestException("Post not found or not updated");
        }
    }
}