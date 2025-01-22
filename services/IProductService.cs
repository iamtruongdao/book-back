using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Product;
using back.models;
using back.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace back.services
{
    public interface IProductService
    {
        public  Task<CreateProductDTO?> AddProduct( CreateProductDTO product);
        public Task<DeleteResult> DeleteProduct(string id);
        public Task<ReplaceOneResult> UpdateProduct( UpdateProductDTO product);
        public Task<Product?> GetProduct(string id);
        public Task<List<Product>> GetAllProduct();
        public Task<PaginatedList<Product>> GetAllFilter(string sortOrder,string currentFilter,string searchString,int? pageNumber,int pageSize );
    }
}