using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using back.DTOs.Author;
using back.DTOs.Product;
using back.models;
using back.Viewmodel;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Slugify;
namespace back.services
{
    public class AuthorService: IAuthorService
    {
    private readonly IMongoCollection<Author> _authors;
    private readonly  ICloundinaryService _cloudinaryService;
    private readonly IMapper _mapper;
        public AuthorService( IMongoClient client, ICloundinaryService cloudinaryService, IMapper mapper)
        {
            var database = client.GetDatabase("ecommerce");
            _authors = database.GetCollection<Author>("Authors");
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }
        public async Task<CreateAuthorDTO?> AddAuthor( CreateAuthorDTO author)
        {
            var slugHelper = new SlugHelper();
            author.Slug = slugHelper.GenerateSlug(author.AuthorName);
             if(!String.IsNullOrEmpty(author.Avatar) ){
                string uploadUrl = await _cloudinaryService.uploadImage(author.Avatar);
                author.Avatar = uploadUrl;
             }
            await _authors.InsertOneAsync(_mapper.Map<Author>(author));
             return author;
        }

       

        public async Task<List<Author>> GetAllAuthor()
        {
            return await _authors.Find(_ => true).ToListAsync();
        }

        public async Task<DeleteResult> DeleteAuthor(string id)
        {
            return await _authors.DeleteOneAsync(product => product.Id == id);
        }
        public async Task<ReplaceOneResult> UpdateAuthor(UpdateAuthorDTO author)
        {
            if(!String.IsNullOrEmpty(author.Avatar)) {
                string uploadUrl = await _cloudinaryService.uploadImage(author.Avatar);
                author.Avatar = uploadUrl;
            }
            return await _authors.ReplaceOneAsync(p => p.Id == author.Id, _mapper.Map<Author>(author));
        }
        public async Task<Author?> GetAuthor(string slug)
        {
            return await _authors.Find(p => p.Slug == slug).FirstOrDefaultAsync();
        }

        public async Task<PaginatedList<Author>> GetAllFilter(string sortOrder, string currentFilter,string searchString, int? pageNumber, int pageSize)
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
            long totalPage = await _authors.Find(filter).CountDocumentsAsync();
            var author = await _authors.Find(filter).Sort(sort).Skip((pageNumber-1)*pageSize).Limit(pageSize).ToListAsync();
            return new PaginatedList<Author>(author,(int)totalPage,pageNumber ?? 1,pageSize );
        }

        public async Task<List<CreateAuthorDTO>?> AddAuthorMany(List<CreateAuthorDTO> author)
        {
            await _authors.InsertManyAsync(_mapper.Map<List<Author>>(author));
            return author;
        }

        public async Task<List<string?>> GetStrings()
        {
            return await _authors.Find(x => true).Project(x => x.Id).ToListAsync();
        }
    }
  
}