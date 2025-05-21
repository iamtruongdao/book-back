using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface ICartRepository
    {
        Task<Cart> CreateCart(string id, CartProductItem item,bool options);
        Task<Cart> FindCartExist(string id, CartProductItem item);
        Task<Cart> FindById(string id);
        Task<Cart> FindByUserId(string user_id);
        Task<Cart> UpdateQuantity(string user_id,string product_id,int quantity);
        Task<Cart> DeleteCart(string user_id,string product_id);
        List<BsonDocument> GetCart(string user_id);
    }

   

    public class CartRepo :  ICartRepository
    {
        private readonly IMongoCollection<Cart> _cart;
        public CartRepo(IMongoClient client, MongoDbSetting setting)
        {
            var db = client.GetDatabase(setting.DatabaseName);
            _cart = db.GetCollection<Cart>("Carts");
        }

        public async Task<Cart> CreateCart(string user_id, CartProductItem item,bool options)
        {
            FilterDefinition<Cart> filter = Builders<Cart>.Filter.Eq("UserId", user_id) ;
            UpdateDefinition<Cart> update = Builders<Cart>.Update.AddToSet<CartProductItem>("CartProducts", item).Inc("CartCountProduct", 1);
            return await  _cart.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Cart> { IsUpsert = options,ReturnDocument= ReturnDocument.After});
        }

        public async Task<Cart> DeleteCart( string user_id,string product_id)
        {
             var filter = Builders<Cart>.Filter.Eq(c => c.UserId,user_id);
            var updateSet = Builders<Cart>.Update.PullFilter(c => c.CartProducts,p => p.ProductId == product_id ).Inc("CartCountProduct",-1);
            return await _cart.FindOneAndUpdateAsync(filter, updateSet);
        }

        public async Task<Cart> FindById(string id)
        {
            return await _cart.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Cart> FindByUserId(string user_id)
        {
            return await _cart.Find(x => x.UserId == user_id).FirstOrDefaultAsync();
            
        }

        public async Task<Cart> FindCartExist(string id, CartProductItem item)
        {
             var filter = Builders<Cart>.Filter.And(
                Builders<Cart>.Filter.Eq("UserId",id),
                Builders<Cart>.Filter.ElemMatch(p => p.CartProducts,a => a.ProductId == item.ProductId)
            ) ;
            return await _cart.Find(filter).FirstOrDefaultAsync();
        }

        public List<BsonDocument> GetCart(string user_id)
        {
            var cartList = _cart.Aggregate().Match(Builders<Cart>.Filter.Eq(c => c.UserId, user_id)).Lookup("Products", "CartProducts.ProductId", "_id", "ProductDetails").Project(BsonDocument.Parse(@"{
                _id:1,
                UserId:{$toString:'$UserId'},
                CartCountProduct:1,
                CartProducts: {
                $map: {
                    input: '$CartProducts',
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
            return cartList;
        }

        public async Task<Cart> UpdateQuantity(string user_id, string product_id,int quantity)
        {
            var filter = Builders<Cart>.Filter.And(
                Builders<Cart>.Filter.Eq("UserId",user_id),
                Builders<Cart>.Filter.Eq("CartProducts.ProductId",product_id)
            ) ;
            var update = Builders<Cart>.Update.Inc<int>("CartProducts.$.Quantity", quantity);
            return await _cart.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Cart> { IsUpsert = true,ReturnDocument = ReturnDocument.After });
        }
    }
}