using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Author;
using BackEnd.DTOs.Categories;
using BackEnd.DTOs.Product;
using BackEnd.models;
using BackEnd.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BackEnd.services
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