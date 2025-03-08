using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using back.DTOs.Categories;

using back.models;
using back.Viewmodel;

using MongoDB.Driver;
using Slugify;
namespace back.services.Categories
{
    public class CategoryService: ICategoriesService
    {
    private readonly IMongoCollection<Category> _categories;

    private readonly IMapper _mapper;
        public CategoryService( IMongoClient client,  IMapper mapper,MongoDbSetting setting)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _categories = database.GetCollection<Category>("Categories");
            _mapper = mapper;
        }
        public async Task<Category?> AddCat( CreateCatDTO cat)
        {
            var catCreate = _mapper.Map<Category>(cat);
            var slugHelper = new SlugHelper();
            catCreate.Slug = slugHelper.GenerateSlug(cat.Name);
            await _categories.InsertOneAsync(catCreate);
             return catCreate;
        }

       

        public async Task<List<Category>> GetAllCat()
        {
            return await _categories.Find(_ => true).ToListAsync();
        }

        public async Task<DeleteResult> DeleteCat(string id)
        {
            return await _categories.DeleteOneAsync(product => product.Id == id);
        }
        public async Task<ReplaceOneResult> UpdateCat(UpdateCatDTO cat)
        {
            
            return await _categories.ReplaceOneAsync(p => p.Id == cat.Id, _mapper.Map<Category>(cat));
        }
        public async Task<Category?> GetCat(string slug)
        {
            return await _categories.Find(p => p.Slug == slug).FirstOrDefaultAsync();
        }

        public async Task<PaginatedList<Category>> GetAllFilter(string sortOrder, string currentFilter,string searchString, int? pageNumber, int pageSize)
        {
            if(!String.IsNullOrEmpty(searchString)) {
                pageNumber = 1;
            } 
            var filter = Builders<Category>.Filter.Empty;
            SortDefinition<Category> sort;
            if(!String.IsNullOrEmpty(currentFilter)) {
                sort = sortOrder.ToLower() == "desc" ? Builders<Category>.Sort.Descending(currentFilter) : Builders<Category>.Sort.Ascending(currentFilter);
            } else {
                sort = Builders<Category>.Sort.Ascending("Id");
            }
            if(!String.IsNullOrEmpty(searchString)) {
                filter = Builders<Category>.Filter.Regex("AuthorName", new MongoDB.Bson.BsonRegularExpression(searchString));
            }            
            long totalPage = await _categories.Find(filter).CountDocumentsAsync();
            var author = await _categories.Find(filter).Sort(sort).Skip((pageNumber-1)*pageSize).Limit(pageSize).ToListAsync();
            return new PaginatedList<Category>(author,(int)totalPage,pageNumber ?? 1,pageSize );
        }

     
      
    }
  
}