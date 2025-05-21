using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Product;
using BackEnd.models;
using BackEnd.Viewmodel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface IProductRepository
    {
        Task<Product> FindById(string id);
        Task<Product> FindBySlug(string slug);
        Task<Product> Create(Product category);
        Task<List<Product>> GetAllProducts();
        Task<List<Product>> GetLimit(int limit);
        Task<ReplaceOneResult> UpdateProduct(Product product);
        Task<DeleteResult> DeleteProduct(string id);
        Task<List<BsonDocument>> GetOneProduct(FilterDefinition<Product> filter);
        Task<(List<BsonDocument>,long)> GetAllFilter(FilterDefinition<Product> filter,SortDefinition<Product> sort,  int pageNumber, int pageSize);
    }
    public class ProductRepo : IProductRepository
    {
        private readonly IMongoCollection<Product> _product;
        
        public ProductRepo(IMongoClient client, MongoDbSetting setting)
        {
            var db = client.GetDatabase(setting.DatabaseName);
            _product = db.GetCollection<Product>("Products");
        }

        public async Task<Product> Create(Product product)
        {
            await _product.InsertOneAsync(product);
            return product;
        }

        public Task<DeleteResult> DeleteProduct(string id)
        {
            return _product.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<Product> FindById(string id)
        {
            return await _product.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Product> FindBySlug(string slug)
        {
            return await _product.Find(x => x.Slug == slug).FirstOrDefaultAsync();

        }
        public async Task<(List<BsonDocument>,long)> GetAllFilter(FilterDefinition<Product> filter,SortDefinition<Product> sort,  int pageNumber, int pageSize)
        {   
             
            long totalPage = await _product.Find(filter).CountDocumentsAsync();
            return  (await _product.Aggregate().Match(filter).Sort(sort).Lookup("Authors","Author","_id","AuthorInfor").Lookup("Categories","Cat","_id","Category").Project(BsonDocument.Parse(@"{
                _id:1,
                ProductPrice: 1,
                ProductName: 1,
                Discount: 1,
                Sold:1,
                ProductQuantity:1,
                AuthorName: {$arrayElemAt:['$AuthorInfor.AuthorName',0]},
                DiscountPrice:{ 
                    $subtract: [ 
                        '$ProductPrice', 
                        { $multiply: [ '$ProductPrice', { $divide: [ '$Discount', 100 ] } ] } 
                    ] 
                },
                Avatar:1,
                Slug:1,
                Category: 1,
                PublicDate:1,
                Author:{$toString:'$Author'},
                ProductDescription:1,   
            }")).Skip((pageNumber - 1)*pageSize).Limit(pageSize).ToListAsync(),totalPage);
        }
        public async Task<List<Product>> GetAllProducts()
        {
            return await _product.Find(_ => true).ToListAsync();
        }

        public async Task<List<Product>> GetLimit(int limit)
        {
           return await _product.Find(_ => true).Limit(limit).ToListAsync();
        }

        public async Task<List<BsonDocument>> GetOneProduct(FilterDefinition<Product> filter)
        {
            return await _product.Aggregate().Match(filter).Lookup("Authors", "Author", "_id", "AuthorInfo").Lookup("Categories", "Cat", "_id", "Category").Project(BsonDocument.Parse(@"{
                _id:1,
                ProductPrice:1,
                Discount:1,
                ProductName:1,
                ProductDescription:1,
                ProductQuantity:1,
                Slug:1,
                Sold:1,
                PublicDate:1,
                DiscountPrice:{ 
                    $subtract: [ 
                        '$ProductPrice', 
                        { $multiply: [ '$ProductPrice', { $divide: [ '$Discount', 100 ] } ] } 
                    ] 
                },
                Translator:1,
                Avatar:1,
                PageNumber:1,
                Author:{$toString:'$Author'},
                AuthorName:{$arrayElemAt:['$AuthorInfo.AuthorName',0]},
                Category:1
            }")).ToListAsync();
        }

        public Task<ReplaceOneResult> UpdateProduct(Product product)
        {
            return _product.ReplaceOneAsync(x => x.Id == product.Id, product);
        }
    }
}