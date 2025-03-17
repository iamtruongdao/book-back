using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.models;
using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface IProductRepository
    {
        public Task<Product> FindById(string id);
        public Task<Product> FindBySlug(string slug);
    }
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _product;
        public  ProductRepository(IMongoClient client, MongoDbSetting setting)
        {
            var db = client.GetDatabase(setting.DatabaseName);
            _product = db.GetCollection<Product>("Products");
        }
        public async Task<Product> FindById(string id)
        {
            return await _product.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Product> FindBySlug(string slug)
        {
            return await _product.Find(x => x.Slug == slug).FirstOrDefaultAsync();

        }
    }
}