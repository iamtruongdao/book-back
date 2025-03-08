using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using back.DTOs.Cart;
using back.models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace back.services
{
    public class CartService : ICartServices
    {
        private readonly IMongoCollection<Cart> _cart;
        public CartService(IMongoClient client,MongoDbSetting setting) {
            var database = client.GetDatabase(setting.DatabaseName);
            _cart = database.GetCollection<Cart>("Carts");
        }
        public async  Task<Cart> AddProductToCart(AddProductToCartDTO product)
        {
            var userCart = await _cart.Find(x => x.UserId == product.UserId).FirstOrDefaultAsync();
            if(userCart is null) {
                return await this.CreateUserCart(product);
            }
             var filter = Builders<Cart>.Filter.And(
                Builders<Cart>.Filter.Eq("UserId",product.UserId),
                Builders<Cart>.Filter.ElemMatch(p => p.CartProduct,a => a.ProductId == product.CartItem.ProductId)
            ) ;
            var userCartProductExist = await _cart.Find(filter).FirstOrDefaultAsync();
            if(userCartProductExist is null) {
                return await this.AddProductToCartExist(product);
            }
            return await this.UpdateUserCartQuantity(product);
        }

        public async Task<Cart> CreateUserCart(AddProductToCartDTO product)
        {
            FilterDefinition<Cart> filter = Builders<Cart>.Filter.Eq("UserId", product.UserId) ;
            UpdateDefinition<Cart> update = Builders<Cart>.Update.AddToSet<CartProductItem>("CartProduct", product.CartItem);
            return await  _cart.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Cart> { IsUpsert = true,ReturnDocument= ReturnDocument.After});
        }
        public async Task<Cart> UpdateUserCartQuantity(AddProductToCartDTO product)
        {
            var filter = Builders<Cart>.Filter.And(
                Builders<Cart>.Filter.Eq("UserId",product.UserId),
                Builders<Cart>.Filter.Eq("CartProduct.ProductId",product.CartItem.ProductId)
            ) ;
            var update = Builders<Cart>.Update.Inc<int>("CartProduct.$.Quantity", product.CartItem.Quantity);
            return await _cart.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Cart> { IsUpsert = true });
        }

        public async Task<Cart> AddProductToCartExist(AddProductToCartDTO product)
        {
            var filter = Builders<Cart>.Filter.Eq("UserId", product.UserId);
            var update = Builders<Cart>.Update.AddToSet<CartProductItem>("CartProduct", product.CartItem);
            return await _cart.FindOneAndUpdateAsync(filter, update);
        }
        public async Task<Cart> DeleteCart(DeleteItemDTO product)
        {
            var filter = Builders<Cart>.Filter.Eq(c => c.UserId,product.UserId);
            var updateSet = Builders<Cart>.Update.PullFilter(c => c.CartProduct,p => p.ProductId == product.ProductId );
            return await _cart.FindOneAndUpdateAsync(filter, updateSet);
        }

        public IEnumerable<CartResponse> GetCart(string user_id)
        {
            var cartList = _cart.Aggregate().Match(Builders<Cart>.Filter.Eq(c => c.UserId,user_id)).Lookup("Products", "CartProduct.ProductId", "_id", "ProductDetails").Project(BsonDocument.Parse(@"{
                _id:{$toString:'$_id'},
                UserId:{$toString:'$UserId'},
                CartProduct: {
                $map: {
                    input: '$CartProduct',
                    as: 'cartItem',
                    in: {
                        ProductId: {$toString:'$$cartItem.ProductId'},
                        Quantity: '$$cartItem.Quantity',
                        ProductDetails: {
                        $let: {
                            vars: {
                                product: {
                                    $arrayElemAt: [
                                        {
                                            $filter: {
                                                input: '$ProductDetails',
                                                as: 'product',
                                                cond: { $eq: ['$$product._id', '$$cartItem.ProductId'] }
                                            }
                                        },
                                        0
                                    ]
                                }
                            },
                            in: {
                                ProductName: '$$product.ProductName',
                                ProductPrice: '$$product.ProductPrice',
                                Discount: {$divide:[
                                    {
                                        $multiply:['$$product.ProductPrice','$$product.Discount']
                                    },100]
                                },
                                Avatar: '$$product.Avatar',
                                Slug: '$$product.Slug',
                            }
                        }
                    }
                    }
                }
            }
            }")).ToList();
            var cartRes = cartList.Select(doc => BsonSerializer.Deserialize<CartResponse>(doc));
            return cartRes;
        }

        public async Task<Cart> IncOrDecProductQuantity(IncOrDecProductQuantityDTO product)
        {
            if (product.Quantity == 0) return  await this.DeleteCart(new DeleteItemDTO
            {
                UserId = product.UserId!,
                ProductId = product.ProductId!
            });
            return await this.UpdateUserCartQuantity(new AddProductToCartDTO
            {
                UserId = product.UserId,
                CartItem = new CartProductItem { ProductId = product.ProductId, Quantity = product.Quantity - product.OldQuantity }
            });
        }

        public async Task<Cart> FindById(string Id)
        {
            return await _cart.Find(x => x.Id == Id).FirstOrDefaultAsync();
        }
    }
}