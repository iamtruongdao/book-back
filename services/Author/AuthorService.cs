using AutoMapper;
using BackEnd.DTOs.Author;
using BackEnd.models;
using BackEnd.Viewmodel;
using BackEnd.Exceptions;
using BackEnd.Repository;
using MongoDB.Driver;
using Slugify;
namespace BackEnd.services
{
    public class AuthorService: IAuthorService
    {
    private readonly IMongoCollection<Author> _authors;
    private readonly IAuthorRepository _authorRepo;
    private readonly ICloundinaryService _cloudinaryService;
    private readonly IMapper _mapper;
        public AuthorService( IMongoClient client, ICloundinaryService cloudinaryService, IMapper mapper,IAuthorRepository authorRepo)
        {
            var database = client.GetDatabase("ecommerce");
            _authors = database.GetCollection<Author>("Authors");
            _cloudinaryService = cloudinaryService;
            _authorRepo = authorRepo;
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
            await _authorRepo.Create(_mapper.Map<Author>(author));
             return author;
        }
        public async Task<List<Author>> GetAllAuthor()
        {
            return await _authorRepo.GetAllAuthors();
        }

        public async Task<DeleteResult> DeleteAuthor(string id)
        {
            return await _authorRepo.DeleteAuthor(id);
        }
        public async Task<ReplaceOneResult> UpdateAuthor(UpdateAuthorDTO author)
        {
            if(!String.IsNullOrEmpty(author.Avatar)) {
                string uploadUrl = await _cloudinaryService.uploadImage(author.Avatar);
                author.Avatar = uploadUrl;
            }
            return await _authorRepo.UpdateAuthor(_mapper.Map<Author>(author));
        }
        public async Task<Author?> GetAuthor(string slug)
        {
           
            var author =  await _authorRepo.FindBySlug(slug);
            if (author is null) throw new NotFoundException("author notfound");
            return author;
        }

        public async Task<PaginatedList<Author>> GetAllFilter(string sortOrder, string currentFilter,string searchString, int? pageNumber, int pageSize)
        {
            return await _authorRepo.GetAllFilter(sortOrder,currentFilter,searchString,pageNumber,pageSize);
        }
        //ko care
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