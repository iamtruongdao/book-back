using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Author;
using back.DTOs.Categories;
using back.DTOs.Product;
using back.models;
using back.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace back.services
{
    public interface ICategoriesService
    {
        public  Task<Category?> AddCat(CreateCatDTO cat);
        public Task<DeleteResult> DeleteCat(string id);
        public Task<ReplaceOneResult> UpdateCat( UpdateCatDTO cat);
        public Task<Category?> GetCat(string id);
        public Task<List<Category>> GetAllCat();
        public Task<PaginatedList<Category>> GetAllFilter(string sortOrder,string currentFilter,string searchString,int? pageNumber,int pageSize );
    }
}