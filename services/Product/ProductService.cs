using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTOs.Inven;
using BackEnd.DTOs.Product;
using BackEnd.models;
using BackEnd.Viewmodel;
using BackEnd.Exceptions;
using BackEnd.Repository;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Slugify;
namespace BackEnd.services
{
    public class ProductService
     : IProductService
    {
        private readonly IMongoCollection<Product> _products;
        private readonly ICloundinaryService _cloudinaryService;
        private readonly IInventoryService _inventoryService;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IProductRepository _productRepo;

        private readonly IMapper _mapper;
        public ProductService(IMongoClient client, ICloundinaryService cloudinaryService, IMapper mapper, MongoDbSetting setting, IInventoryService inventoryService, ICategoryRepository categoryRepository, IProductRepository productRepo)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _products = database.GetCollection<Product>("Products");
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
            _inventoryService = inventoryService;
            _categoryRepo = categoryRepository;
            _productRepo = productRepo;
        }
        public async Task<CreateProductDTO?> AddProduct(CreateProductDTO product)
        {

            var slugHelper = new SlugHelper();
            product.Slug = slugHelper.GenerateSlug(product.ProductName);
            string uploadUrl = await _cloudinaryService.uploadImage(product.Avatar ?? "");
            product.Avatar = uploadUrl;
            var mapProduct = _mapper.Map<Product>(product);
            await _productRepo.Create(mapProduct);
            var addStockToInventory = new AddStockToInventoryDTO
            {
                ProductId = mapProduct.Id,
                Stock = mapProduct.ProductQuantity
            };
            await _inventoryService.AddStockToInventory(addStockToInventory);

            return product;
        }
        public async Task<List<Product>> GetAllProduct()
        {
            return await _productRepo.GetAllProducts();
        }
        public async Task<List<Product>> GetSliderProduct(int limit)
        {
            return await _productRepo.GetLimit(limit);
        }

        public async Task<DeleteResult> DeleteProduct(string id)
        {
            return await _productRepo.DeleteProduct(id);
        }
        public async Task<ReplaceOneResult> UpdateProduct(UpdateProductDTO product)
        {
            var slugHelper = new SlugHelper();
            product.Slug = slugHelper.GenerateSlug(product.ProductName);
            if (!String.IsNullOrEmpty(product.Avatar))
            {
                string uploadUrl = await _cloudinaryService.uploadImage(product.Avatar);
                product.Avatar = uploadUrl;
            }
            return await _productRepo.UpdateProduct(_mapper.Map<Product>(product));
        }
        public async Task<ProductDTO> GetProduct(string slug)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Slug, slug);
            var products = await _productRepo.GetOneProduct(filter);
            if (products is null) throw new NotFoundException("product is notfound");
            var newProducts = products.Select(doc => BsonSerializer.Deserialize<ProductDTO>(doc)).First();  
            return newProducts;
        }
        public async Task<PaginatedList<ProductDTO>> GetAllFilter(string sortOrder, string currentFilter, string searchString, string category, int pageNumber, int pageSize, decimal minPrice, decimal maxPrice)
        {
           
           var buildFilter = Builders<Product>.Filter;
            var filters = new List<FilterDefinition<Product>>(); // ✅ danh sách filter
            SortDefinition<Product> sort;
            var orderBy = Builders<Product>.Sort;

            // Sort
            if (!string.IsNullOrEmpty(currentFilter))
            {
                sort = sortOrder?.ToLower() == "desc"
                    ? orderBy.Descending(currentFilter)
                    : orderBy.Ascending(currentFilter);
            }
            else
            {
                sort = orderBy.Ascending("Id");
            }
            Console.WriteLine(searchString);
            // Search text
            if (!string.IsNullOrEmpty(searchString))
            {
                filters.Add(buildFilter.Regex(p => p.ProductName, new BsonRegularExpression(searchString, "i")));
            }

            // Price range
            if (minPrice != 0 && maxPrice != 0)
            {
                filters.Add(buildFilter.Gte(x => x.ProductPrice, minPrice));
                filters.Add(buildFilter.Lte(x => x.ProductPrice, maxPrice));
            }

            // Category
            if (!string.IsNullOrEmpty(category))
            {
                var cate = await _categoryRepo.FindBySlug(category);
                filters.Add(buildFilter.AnyEq(x => x.Cat, cate.Id!));
            }
            
            // Combine all filters
            var filter = filters.Any() ? buildFilter.And(filters) : buildFilter.Empty;
          
            var (products, totalPage) = await _productRepo.GetAllFilter(filter, sort, pageNumber, pageSize);

            var convert = products.Select(doc => BsonSerializer.Deserialize<ProductDTO>(doc)).ToList();

            return new PaginatedList<ProductDTO>(convert, (int)totalPage, pageNumber, pageSize);

        }

        public async Task<List<CreateProductDTO>?> AddProductMany(List<CreateProductDTO> product)
        {
            await _products.InsertManyAsync(product.Select(p => _mapper.Map<Product>(p)));
            foreach (var item in product)
            {
                await _inventoryService.AddStockToInventory(new AddStockToInventoryDTO
                {
                    ProductId = item.Id!.ToString(),
                    Stock = item.ProductQuantity    
                });
            }
            return product;
        }

        public async Task<ProductDTO> GetProductByAuthor(string author)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.And(builder.Eq(x => x.Author, author));
            var products = await _productRepo.GetOneProduct(filter);
            if (products is null) throw new NotFoundException("product is not found");
            var convert = products.Select(doc => BsonSerializer.Deserialize<ProductDTO>(doc)).ToList().AsQueryable().OrderByDescending(x => x.Sold).First();
            return convert;
        }

        public async Task<PaginatedList<ProductDTO>> GetProductWaitPublish(int pageSize = 10, int pageNumber = 1)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.IsPublic, false);
            var (products,totalPage) = await  _productRepo.GetAllFilter(filter, Builders<Product>.Sort.Descending("PublicDate"), pageNumber, pageSize);
            var convert = products.Select(doc => BsonSerializer.Deserialize<ProductDTO>(doc)).ToList();
            return new PaginatedList<ProductDTO>(convert, (int)totalPage, pageNumber, pageSize);
        }

        public async Task<PaginatedList<ProductDTO>> GetTopProduct(int year,int pageSize = 5, int pageNumber = 1)
        {
            
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1);

            var builder = Builders<Product>.Filter;
            var filter = builder.Gte(x => x.PublicDate, startDate) & builder.Lt(x => x.PublicDate, endDate);
            var (products,totalPage) = await  _productRepo.GetAllFilter(filter, Builders<Product>.Sort.Descending("Sold"), pageNumber, pageSize);
            var convert = products.Select(doc => BsonSerializer.Deserialize<ProductDTO>(doc)).ToList();
            return new PaginatedList<ProductDTO>(convert, (int)totalPage, pageNumber, pageSize);
        }
    }
}