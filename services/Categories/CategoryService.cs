using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using BackEnd.DTOs.Categories;

using BackEnd.models;
using BackEnd.Viewmodel;
using BackEnd.Repository;
using MongoDB.Driver;
using Slugify;
namespace BackEnd.services.Categories
{
    public class CategoryService: ICategoriesService
    {
    private readonly IMongoCollection<Category> _categories;
    private readonly ICategoryRepository _categoriRepo;

    private readonly IMapper _mapper;
        public CategoryService( IMongoClient client,  IMapper mapper,MongoDbSetting setting,ICategoryRepository categoryRepo)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _categories = database.GetCollection<Category>("Categories");
            _categoriRepo = categoryRepo;
            _mapper = mapper;
        }
        public async Task<Category?> AddCat( CreateCatDTO cat)
        {
            var catCreate = _mapper.Map<Category>(cat);
            await _categoriRepo.Create(catCreate);
            return catCreate;
        }
        public async Task<List<Category>> GetAllCat()
        {
            return await _categoriRepo.GetAllCategories();
        }

        public async Task<DeleteResult> DeleteCat(string id)
        {
            return await _categoriRepo.DeleteCategory(id);
        }
        public async Task<ReplaceOneResult> UpdateCat(UpdateCatDTO cat)
        {
            
            return await _categoriRepo.UpdateCategory(_mapper.Map<Category>(cat));
        }
        public async Task<Category?> GetCat(string slug)
        {
            return await _categoriRepo.FindBySlug(slug);
        }

        public async Task<PaginatedList<Category>> GetAllFilter(string sortOrder, string currentFilter,string searchString, int? pageNumber, int pageSize)
        {
            return await _categoriRepo.GetAllFilter(sortOrder,currentFilter,searchString,pageNumber,pageSize);
        }

     
      
    }
  
}