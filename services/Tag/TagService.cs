using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Viewmodel;
using BackEnd.DTOs.Tag;
using BackEnd.Exceptions;
using BackEnd.models;
using BackEnd.Repository;

namespace BackEnd.services.Tag
{
    public class TagService :ITagService
    {
        private readonly ITagRepository _repository;
        private readonly IMapper _mapper;

        public TagService(ITagRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public Task<PaginatedList<Tags>> GetAllAsync(int pageSize, int pageNumber) => _repository.GetAllAsync(pageSize, pageNumber);
   

    public Task<Tags?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public async Task<TagDto> GetBySlugAsync(string slug,int pageSize,int pageNumber)   {
       
        var result = await  _repository.GetBySlugAsync(slug,pageSize,pageNumber);
        if (result is null) throw new NotFoundException("not found");
        return result;
    }

        public async Task<Tags> CreateAsync(CreateTagDto tag)
        {
            var result = await _repository.CreateAsync(_mapper.Map<Tags>(tag));
            if (String.IsNullOrEmpty(result.Id)) throw new BadRequestException("create failed");
            return result;
        }

        public async Task UpdateAsync(UpdateTagDto tag)
        {
            var result = await _repository.UpdateAsync(_mapper.Map<Tags>(tag));
            if (result.MatchedCount == 0) throw new BadRequestException("update failed");
        }

        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
        }
}