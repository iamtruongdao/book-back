using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Cart;
using back.models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace back.services
{
    public interface ICartServices
    {
        public Task<Cart> CreateUserCart(AddProductToCartDTO product);
        public Task<Cart> UpdateUserCartQuantity(AddProductToCartDTO product);
        public Task<Cart> AddProductToCart(AddProductToCartDTO product);
        public Task<Cart> AddProductToCartExist(AddProductToCartDTO product);
        public Task<Cart> IncOrDecProductQuantity(IncOrDecProductQuantityDTO product);
        public IEnumerable<CartResponse> GetCart( string user_id);
        public Task<Cart> DeleteCart( DeleteItemDTO product);
        public Task<Cart> FindById( string Id);
    }
}