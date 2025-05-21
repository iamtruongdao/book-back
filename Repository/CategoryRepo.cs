using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.models;
using BackEnd.Viewmodel;
using MongoDB.Driver;
using Slugify;

namespace BackEnd.Repository
{
    public interface ICategoryRepository
    {
        Task<Category> FindById(string id);
        Task<Category> FindBySlug(string slug);
        Task<Category> Create(Category category);
        Task<List<Category>> GetAllCategories();
        Task<ReplaceOneResult> UpdateCategory(Category category);
        Task<DeleteResult> DeleteCategory(string id);
        Task<PaginatedList<Category>> GetAllFilter(string sortOrder, string currentFilter, string searchString, int? pageNumber, int pageSize);
    }
    public class CategoryRepo : ICategoryRepository
    {
        private readonly IMongoCollection<Category> _categories;

   
        public CategoryRepo( IMongoClient client, MongoDbSetting setting)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _categories = database.GetCollection<Category>("Categories");
        }

        public async Task<Category> Create(Category category)
        {
            var slugHelper = new SlugHelper();
            category.Slug = slugHelper.GenerateSlug(category.Name);
            await _categories.InsertOneAsync(category);
            return category;
        }

        public async Task<DeleteResult> DeleteCategory(string id)
        {
            return await _categories.DeleteOneAsync(product => product.Id == id);
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _categories.Find(_ => true).ToListAsync();
        }

        public Task<Category> FindById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<ReplaceOneResult> UpdateCategory(Category category)
        {
            return await _categories.ReplaceOneAsync(p => p.Id == category.Id, category);
        }

        public async Task<Category> FindBySlug(string slug)
        {
            return await _categories.Find(p => p.Slug == slug).FirstOrDefaultAsync();
        }

        public async Task<PaginatedList<Category>> GetAllFilter(string sortOrder, string currentFilter, string searchString, int? pageNumber, int pageSize)
        {
           var filterBuilder = Builders<Category>.Filter;
            var filters = new List<FilterDefinition<Category>>();

            // Nếu có searchString, thêm điều kiện tìm kiếm theo Name
            if (!string.IsNullOrEmpty(searchString))
            {
                pageNumber = 1;
                filters.Add(filterBuilder.Regex("Name", new MongoDB.Bson.BsonRegularExpression(searchString, "i"))); // "i" để ignore case
            }

            // Nếu không có điều kiện nào thì mặc định Empty, còn không thì combine tất cả với AND
            var filter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

            // Xử lý sort
            SortDefinition<Category> sort;
            if (!string.IsNullOrEmpty(currentFilter))
            {
                sort = sortOrder.ToLower() == "desc" 
                    ? Builders<Category>.Sort.Descending(currentFilter) 
                    : Builders<Category>.Sort.Ascending(currentFilter);
            }
            else
            {
                sort = Builders<Category>.Sort.Ascending("Id");
            }        
            long totalPage = await _categories.Find(filter).CountDocumentsAsync();
            var cat = await _categories.Find(filter).Sort(sort).Skip((pageNumber-1)*pageSize).Limit(pageSize).ToListAsync();
            return new PaginatedList<Category>(cat,(int)totalPage,pageNumber ?? 1,pageSize );
        }
    }
}