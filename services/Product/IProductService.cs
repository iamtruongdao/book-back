using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Product;
using BackEnd.models;
using BackEnd.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BackEnd.services
{
    public interface IProductService
    {
          Task<CreateProductDTO?> AddProduct( CreateProductDTO product);
          Task<List<CreateProductDTO>?> AddProductMany( List<CreateProductDTO> product);
         Task<DeleteResult> DeleteProduct(string id);
         Task<ReplaceOneResult> UpdateProduct( UpdateProductDTO product);
         Task<ProductDTO> GetProduct(string id);
         Task<ProductDTO> GetProductByAuthor(string author);
    
         Task<List<Product>> GetAllProduct();
         Task<List<Product>> GetSliderProduct(int limit);
         Task<PaginatedList<ProductDTO>> GetProductWaitPublish(int pageSize = 10,int pageNumber = 1);
         Task<PaginatedList<ProductDTO>> GetTopProduct(int year, int pageSize = 5,int pageNumber = 1);
         Task<PaginatedList<ProductDTO>> GetAllFilter(string sortOrder,string currentFilter,string searchString,string category,int pageNumber,int pageSize,decimal minPrice,decimal maxPrice );
    }
}