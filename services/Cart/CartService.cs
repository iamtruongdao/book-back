using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTOs.Cart;
using BackEnd.models;
using BackEnd.Exceptions;
using BackEnd.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BackEnd.services
{
    public class CartService : ICartServices
    {
     
        private readonly ICartRepository _cartRepo;
        private readonly IMapper _mapper;
        public CartService( ICartRepository cartRepo, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _mapper = mapper;
        }
        public async  Task<Cart> AddProductToCart(AddProductToCartDTO product)
        {
            Console.WriteLine("in that casse");
            var userCart = await _cartRepo.FindByUserId(product.UserId!);
            if(userCart is null) {
                return await _cartRepo.CreateCart(product.UserId!,product.CartItem,true);
            }
            var userCartProductExist = await _cartRepo.FindCartExist(product.UserId!,product.CartItem);
            if(userCartProductExist is null) {
                Console.WriteLine("in that casse");
                return await _cartRepo.CreateCart(product.UserId!, product.CartItem, false);
            }
            Console.WriteLine("in that casse >>>");
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
           var cartList = _cartRepo.GetCart(user_id);
           if(cartList.Count() == 0) throw new NotFoundException("Cart not found");
            var cartRes = cartList.Select(doc => BsonSerializer.Deserialize<CartResponse>(doc)).First();
            return cartRes;
        }
        public async Task<Cart> IncOrDecProductQuantity(IncOrDecProductQuantityDTO product)
        {
            if (product.Quantity == 0) return  await _cartRepo.DeleteCart(product.UserId!,product.ProductId!);
            return await _cartRepo.UpdateQuantity(product.UserId!,product.ProductId!, product.Quantity - product.OldQuantity);
        }

    }
}