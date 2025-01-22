using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using back.DTOs.Product;
using back.models;
using back.Viewmodel;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Slugify;
namespace back.services
{
    public class ProductService
     : IProductService
    {
    private readonly IMongoCollection<Product> _products;
    private readonly  ICloundinaryService _cloudinaryService;
  
    private readonly IMapper _mapper;
        public ProductService(IMongoClient client, ICloundinaryService cloudinaryService, IMapper mapper,MongoDbSetting setting)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _products = database.GetCollection<Product>("Products");
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }
        public async Task<CreateProductDTO?> AddProduct( CreateProductDTO product)
        {
            
            var slugHelper = new SlugHelper();
            product.Slug = slugHelper.GenerateSlug(product.ProductName);
            string uploadUrl = await _cloudinaryService.uploadImage(product.Avatar);
            product.Avatar = uploadUrl;
            await _products.InsertOneAsync(_mapper.Map<Product>(product));
            return product;
        }

       

        public async Task<List<Product>> GetAllProduct()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        public async Task<DeleteResult> DeleteProduct(string id)
        {
            return await _products.DeleteOneAsync(product => product.Id == id);
        }
        public async Task<ReplaceOneResult> UpdateProduct(UpdateProductDTO product)
        {
            if(!String.IsNullOrEmpty(product.Avatar)) {
                string uploadUrl = await _cloudinaryService.uploadImage(product.Avatar);
                product.Avatar = uploadUrl;
            }
            return await _products.ReplaceOneAsync(p => p.Id == product.Id, _mapper.Map<Product>(product));
        }
        public async Task<Product?> GetProduct(string slug)
        {
            return await _products.Find(p => p.Slug == slug).FirstOrDefaultAsync();
        }

        public async Task<PaginatedList<Product>> GetAllFilter(string sortOrder, string currentFilter,string searchString, int? pageNumber, int pageSize)
        {
            if(!String.IsNullOrEmpty(searchString)) {
                pageNumber = 1;
            } 
            var filter = Builders<Product>.Filter.Empty;
            SortDefinition<Product> sort;
            if(!String.IsNullOrEmpty(currentFilter)) {
                sort = sortOrder.ToLower() == "desc" ? Builders<Product>.Sort.Descending(currentFilter) : Builders<Product>.Sort.Ascending(currentFilter);
            } else {
                sort = Builders<Product>.Sort.Ascending("Id");
            }
            if(!String.IsNullOrEmpty(searchString)) {
                filter = Builders<Product>.Filter.Text( searchString);
            }            
           long totalPage = await _products.Find(filter).CountDocumentsAsync();
            var products = await _products.Find(filter).Sort(sort).Skip((pageNumber-1)*pageSize).Limit(pageSize).ToListAsync();
            return new PaginatedList<Product>(products,(int)totalPage,pageNumber ?? 1,pageSize );
        }
    }
}