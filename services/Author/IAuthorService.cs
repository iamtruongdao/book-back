using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Author;
using back.DTOs.Product;
using back.models;
using back.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace back.services
{
    public interface IAuthorService
    {
        public Task<CreateAuthorDTO?> AddAuthor(CreateAuthorDTO Author);
        public Task<DeleteResult> DeleteAuthor(string id);
        public Task<ReplaceOneResult> UpdateAuthor(UpdateAuthorDTO Author);
        public Task<Author?> GetAuthor(string id);
        public Task<List<Author>> GetAllAuthor();
        public Task<PaginatedList<Author>> GetAllFilter(string sortOrder, string currentFilter, string searchString, int? pageNumber, int pageSize);
         public  Task<List<CreateAuthorDTO>?> AddAuthorMany( List<CreateAuthorDTO> author);
         public  Task<List<string?>> GetStrings( );
    }
}