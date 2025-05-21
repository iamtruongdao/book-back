using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Viewmodel;
using BackEnd.DTOs.Tag;
using BackEnd.models;

namespace BackEnd.services.Tag
{
    public interface ITagService
    {
        
        Task<PaginatedList<Tags>> GetAllAsync(int pageSize, int pageNumber);
        Task<Tags?> GetByIdAsync(string id);
        Task<TagDto> GetBySlugAsync(string slug,int pageSize,int pageNumber);
        Task<Tags> CreateAsync(CreateTagDto tag);
        Task UpdateAsync( UpdateTagDto tag);
        Task DeleteAsync(string id);
    }
}