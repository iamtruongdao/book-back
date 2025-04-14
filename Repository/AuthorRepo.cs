using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using back.models;
using back.Viewmodel;
using MongoDB.Driver;
using Slugify;

namespace BackEnd.Repository
{
    public interface IAuthorRepository
    {
        Task<Author> Create(Author category);
        Task<Author> FindById(string id);
        Task<Author> FindBySlug(string slug);
        Task<List<Author>> GetAllAuthors();
        Task<ReplaceOneResult> UpdateAuthor(Author category);
        Task<DeleteResult> DeleteAuthor(string id);
        Task<PaginatedList<Author>> GetAllFilter(string sortOrder, string currentFilter, string searchString, int? pageNumber, int pageSize);
    }
    public class AuthorRepo : IAuthorRepository
    {
        private readonly IMongoCollection<Author> _author;

   
        public AuthorRepo( IMongoClient client, MongoDbSetting setting)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _author = database.GetCollection<Author>("Authors");
        }

        public async Task<Author> Create(Author author)
        {
            await _author.InsertOneAsync(author);
            return author;
        }

        public async Task<DeleteResult> DeleteAuthor(string id)
        {
            return await _author.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<Author>> GetAllAuthors()
        {
            return await _author.Find(_ => true).ToListAsync();
        }

        public async Task<Author> FindById(string id)
        {
            return await _author.Find(p => p.Id == id).FirstOrDefaultAsync();

        }

        public async Task<ReplaceOneResult> UpdateAuthor(Author author)
        {
            return await _author.ReplaceOneAsync(p => p.Id == author.Id, author);
        }

        public async Task<Author> FindBySlug(string slug)
        {
            return await _author.Find(p => p.Slug == slug).FirstOrDefaultAsync();
        }

        public async Task<PaginatedList<Author>> GetAllFilter(string sortOrder, string currentFilter, string searchString, int? pageNumber, int pageSize)
        {
            if(!String.IsNullOrEmpty(searchString)) {
                pageNumber = 1;
            } 
            var filter = Builders<Author>.Filter.Empty;
            SortDefinition<Author> sort;
            if(!String.IsNullOrEmpty(currentFilter)) {
                sort = sortOrder.ToLower() == "desc" ? Builders<Author>.Sort.Descending(currentFilter) : Builders<Author>.Sort.Ascending(currentFilter);
            } else {
                sort = Builders<Author>.Sort.Ascending("Id");
            }
            if(!String.IsNullOrEmpty(searchString)) {
                filter = Builders<Author>.Filter.Regex("AuthorName", new MongoDB.Bson.BsonRegularExpression(searchString));
            }            
            long totalPage = await _author.Find(filter).CountDocumentsAsync();
            var author = await _author.Find(filter).Sort(sort).Skip((pageNumber-1)*pageSize).Limit(pageSize).ToListAsync();
            return new PaginatedList<Author>(author,(int)totalPage,pageNumber ?? 1,pageSize );
        }
    }
}