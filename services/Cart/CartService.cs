using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTOs.Cart;
using BackEnd.models;

using BackEnd.Repository;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BackEnd.services
{
    public class CartService : ICartServices
    {
     
        private readonly ICartRepository _cartRepo;
        private readonly IMemoryCache _cache;
        private readonly IProductRepository _productRepository;
        public CartService(ICartRepository cartRepo, IMemoryCache cache, IProductRepository productRepository)
        {
            _cartRepo = cartRepo;
            _cache = cache;
            _productRepository = productRepository;
        }
        public async  Task<Cart> AddProductToCart(AddProductToCartDTO product)
        {
            var cacheKey = $"cart_price_{product.UserId}";
            var userPriceDict = _cache.Get<Dictionary<string, decimal>>(cacheKey) 
                                ?? new Dictionary<string, decimal>();
            var productKey = product.CartItem.ProductId;
            var userCart = await _cartRepo.FindByUserId(product.UserId!);
            if(userCart is null) {
                if (!userPriceDict.ContainsKey(productKey!))
                {
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24), // Tự động xóa sau 24h
                        Priority = CacheItemPriority.Normal
                    };
                    var currentProduct = await _productRepository.FindById(product.CartItem.ProductId!);
                    userPriceDict[productKey!] = currentProduct.ProductPrice  ;
                    // Cập nhật cache với dictionary mới
                    _cache.Set(cacheKey, userPriceDict, cacheOptions);
                }
                return await _cartRepo.CreateCart(product.UserId!, product.CartItem, true);
            }
            var userCartProductExist = await _cartRepo.FindCartExist(product.UserId!,product.CartItem);
            if(userCartProductExist is null) {
                if (!userPriceDict.ContainsKey(productKey!))
                {
                    var currentProduct = await _productRepository.FindById(product.CartItem.ProductId!);
                    userPriceDict[productKey!] = currentProduct.ProductPrice  ;
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24), // Tự động xóa sau 24h
                        Priority = CacheItemPriority.Normal
                    };
                    // Cập nhật cache với dictionary mới
                    _cache.Set(cacheKey, userPriceDict, cacheOptions);
                }
                return await _cartRepo.CreateCart(product.UserId!, product.CartItem, false);
            }
            return await _cartRepo.UpdateQuantity(product.UserId!, product.CartItem.ProductId!, product.CartItem.Quantity);
        }
        public async Task<Cart> UpdateUserCartQuantity(AddProductToCartDTO product)
        {
            return await _cartRepo.UpdateQuantity(product.UserId!,product.CartItem.ProductId!,product.CartItem.Quantity);
        }
        public async Task<Cart> DeleteCart(DeleteItemDTO product)
        {
            return await _cartRepo.DeleteCart(product.UserId!,product.ProductId!);
        }
        public CartResponse GetCart(string user_id)
        {
           var cartList =  _cartRepo.GetCart(user_id);
            if (cartList == null || !cartList.Any())
                return new CartResponse() ;

            var cartRes = cartList.Select(doc => BsonSerializer.Deserialize<CartResponse>(doc)).First();
        
        // Nếu cart không có products, return empty cart
            if (cartRes.CartProducts == null || !cartRes.CartProducts.Any())
            {
                cartRes.CartCountProduct = 0;
                return cartRes;
            }

            // Lấy locked prices từ cache
            var cacheKey = $"cart_price_{user_id}";
            var userPriceDict = _cache.Get<Dictionary<string, decimal>>(cacheKey) 
                            ?? new Dictionary<string, decimal>();
            // Update prices với locked prices
            if (userPriceDict != null)
            {
                foreach (var cartProduct in cartRes.CartProducts)
                {
                    if (cartProduct.ProductDetails != null && cartProduct.ProductId != null)
                    {
                        var productKey = cartProduct.ProductId;
                        if (userPriceDict.TryGetValue(productKey, out decimal lockedPrice))
                        {
                            cartProduct.ProductDetails.ProductPrice = lockedPrice;
                        }
                    }
                }
            }
            return cartRes;
        }
        public async Task<Cart> IncOrDecProductQuantity(IncOrDecProductQuantityDTO product)
        {
            if (product.Quantity == 0) return  await _cartRepo.DeleteCart(product.UserId!,product.ProductId!);
            return await _cartRepo.UpdateQuantity(product.UserId!,product.ProductId!, product.Quantity - product.OldQuantity);
        }

    }
}