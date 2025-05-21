using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Author;
using BackEnd.DTOs.Product;
using BackEnd.models;
using BackEnd.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BackEnd.services
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