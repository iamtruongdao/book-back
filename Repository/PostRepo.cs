using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BackEnd.Viewmodel;
using BackEnd.DTOs.Posts;
using BackEnd.models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface IPostRepository
    {
        Task<PaginatedList<PostDto>> GetAllPosts(int pageNumber, int pageSize);
        Task<Post> FindById(string id);
        Task<PostDto> FindBySLug(string slug);
        Task<Post> CreatePostAsync(Post post);
        Task<List<Post>> CreatePostMany(List<Post> post);
        Task<ReplaceOneResult> UpdatePostAsync(Post post);
        Task<DeleteResult> DeletePostAsync(string id);
        Task<List<Post>> FindByTags(string tags);

    }
    public class PostRepo : IPostRepository
    {
        private readonly IMongoCollection<Post> _post;
        public PostRepo(IMongoClient client, MongoDbSetting setting)
        {
            var db = client.GetDatabase(setting.DatabaseName);
            _post = db.GetCollection<Post>("Posts");
        }
        public async Task<Post> CreatePostAsync(Post post)
        {
            await _post.InsertOneAsync(post);
            return post;
        }

        public async Task<DeleteResult> DeletePostAsync(string id)
        {
            return await _post.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<PaginatedList<PostDto>> GetAllPosts(int pageNumber, int pageSize)
        {
            var totalPage = await _post.CountDocumentsAsync(x => true);
            var res = await  _post.Aggregate().Lookup<Tags,PostDto>("Tags","TagId","_id","Tag").Skip((pageNumber-1)*pageSize).Limit(pageSize).ToListAsync();
            return new PaginatedList<PostDto>(res, (int)totalPage, pageNumber, pageSize);
        }
        

        public Task<Post> FindById(string id)
        {
            return _post.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<PostDto> FindBySLug(string SLug)
        {
            var filter = Builders<Post>.Filter.Eq(x => x.Slug, SLug);
            return await  _post.Aggregate().Match(filter).Lookup<Tags,PostDto>("Tags","TagId","_id","Tag").Project<PostDto>( BsonDocument.Parse(@"{
                TagId:0
            }")).FirstOrDefaultAsync();
        }

        public async Task<ReplaceOneResult> UpdatePostAsync(Post post)
        {
            return await _post.ReplaceOneAsync(x => x.Id == post.Id, post);
        }

       
        public Task<List<Post>> CreatePostMany(List<Post> post)
        {
            return _post.InsertManyAsync(post).ContinueWith(t => post);
        }

        public async Task<List<Post>> FindByTags(string tags)
        {
            return await _post.Find(x => x.TagId == tags).ToListAsync(); 
        }
    }
}