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
    
        public Task<Cart> UpdateUserCartQuantity(AddProductToCartDTO cart);
        public Task<Cart> AddProductToCart(AddProductToCartDTO cart);
    
        public Task<Cart> IncOrDecProductQuantity(IncOrDecProductQuantityDTO cart);
        public CartResponse GetCart( string user_id);
        public Task<Cart> DeleteCart( DeleteItemDTO cart);
    }
}