using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using back.DTOs.Inventory;
using back.DTOs.Product;
using back.models;
using back.Viewmodel;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Slugify;
namespace back.services
{
    public class ProductService
     : IProductService
    {
    private readonly IMongoCollection<Product> _products;
    private readonly  ICloundinaryService _cloudinaryService;
    private readonly  IInventoryService _inventoryService;
    private readonly  ICategoriesService _categoriesService;
  
    private readonly IMapper _mapper;
        public ProductService(IMongoClient client, ICloundinaryService cloudinaryService, IMapper mapper, MongoDbSetting setting, IInventoryService inventoryService, ICategoriesService categoriesService)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _products = database.GetCollection<Product>("Products");
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
            _inventoryService = inventoryService;
            _categoriesService = categoriesService;
        }
        public async Task<CreateProductDTO?> AddProduct( CreateProductDTO product)
        {
            
            var slugHelper = new SlugHelper();
            product.Slug = slugHelper.GenerateSlug(product.ProductName);
            string uploadUrl = await _cloudinaryService.uploadImage(product.Avatar??"");
            product.Avatar = uploadUrl;
            var mapProduct = _mapper.Map<Product>(product);
            await _products.InsertOneAsync(mapProduct);
            
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
            return await _products.Find(_ => true).ToListAsync();
        }
        public async Task<List<Product>> GetSliderProduct(int limit)
        {
            return await _products.Find(_ => true).Limit(limit).ToListAsync();
        }

        public async Task<DeleteResult> DeleteProduct(string id)
        {
            return await _products.DeleteOneAsync(product => product.Id == id);
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
            return await _products.ReplaceOneAsync(p => p.Id == product.Id, _mapper.Map<Product>(product));
        }
        public async Task<ProductDTO> GetProduct(string slug)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Slug, slug);
            var products = await _products.Aggregate().Match(filter).Lookup("Authors", "Author", "_id", "AuthorInfo").Lookup("Categories","Cat","_id","Cat").Project(BsonDocument.Parse(@"{
                _id:{$toString:'$_id'},
                ProductPrice:1,
                Discount:1,
                ProductName:1,
                ProductDescription:1,
                ProductQuantity:1,
                Slug:1,
                Translator:1,
                Avatar:1,
                AuthorName:{$arrayElemAt:['$AuthorInfo.AuthorName',0]},
                Cat:1
            }")).ToListAsync();
            var newProducts = products.Select(doc => BsonSerializer.Deserialize<ProductDTO>(doc)).First();
            return newProducts;
        }
         public async Task<Product?> GetProductById(string id)
        {
            return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }
        public async Task<PaginatedList<ProductDTO>> GetAllFilter(string sortOrder, string currentFilter,string searchString,string category, int pageNumber, int pageSize,decimal minPrice,decimal maxPrice)
        
        {
            var buildFilter = Builders<Product>.Filter;
            
            if (!String.IsNullOrEmpty(searchString))
            {
                pageNumber = 1;
            }
            var filter = buildFilter.Empty;
            SortDefinition<Product> sort;
            var orderBy = Builders<Product>.Sort;
          
            if (!String.IsNullOrEmpty(currentFilter))
            {
                sort = sortOrder?.ToLower() == "desc" ? orderBy.Descending(currentFilter) : orderBy.Ascending(currentFilter);
            }
            else
            {
                sort = Builders<Product>.Sort.Ascending("Id");
            }
            if(!String.IsNullOrEmpty(searchString)) {
                filter = buildFilter.Text(searchString);
            }
            if (minPrice != 0 && maxPrice != 0)
            {
                filter = Builders<Product>.Filter.And(buildFilter.Gte(x => x.ProductPrice,minPrice),buildFilter.Lte(x => x.ProductPrice,maxPrice));
                
            }
            if (!String.IsNullOrEmpty(category))
            {
                var cate = await _categoriesService.GetCat(category); 
                filter = buildFilter.AnyEq(x => x.Cat, cate!.Id);
            }
           long totalPage = await _products.Find(filter).CountDocumentsAsync();
            var products = await _products.Aggregate().Match(filter).Sort(sort).Lookup("Authors","Author","_id","AuthorInfor").Lookup("Categories","Cat","_id","Cat").Project(BsonDocument.Parse(@"{
                _id:{$toString:'$_id'},
                ProductPrice: 1,
                ProductName: 1,
                Discount: 1,
                ProductQuantity:1,
                AuthorName: {$arrayElemAt:['$AuthorInfor.AuthorName',0]},
                Avatar:1,
                Slug:1,
                Cat: 1,
                Author:{$toString:'$Author'},
                ProductDescription:1,

                
            }")).Skip((pageNumber-1)*pageSize).Limit(pageSize).ToListAsync();
           
            var convert = products.Select(doc => BsonSerializer.Deserialize<ProductDTO>(doc)).ToList();
            return new PaginatedList<ProductDTO>(convert,(int)totalPage,pageNumber,pageSize );
        }

        public async Task<List<CreateProductDTO>?> AddProductMany(List<CreateProductDTO> product)
        {
            await _products.InsertManyAsync(product.Select(p => _mapper.Map<Product>(p)));
            foreach (var item in product)
            {
                await _inventoryService.AddStockToInventory(new AddStockToInventoryDTO
                {
                    ProductId = item.Id.ToString(),
                    Stock = item.ProductQuantity
                });
            }
            return product;
        }
    }
}