using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface ICartRepository
    {
        Task<Cart> FindById(string id);
        Task<Cart> UpdateQuantity(string user_id,string product_id,int quantity);
        Task<Cart> DeleteCart(string id,string user_id,string product_id);
        IEnumerable<Cart> GetCart(string user_id);
    }

   

    public class CartRepo :  ICartRepository
    {
        private readonly IMongoCollection<Cart> _cart;
        public CartRepo(IMongoClient client, MongoDbSetting setting)
        {
            var db = client.GetDatabase(setting.DatabaseName);
            _cart = db.GetCollection<Cart>("Carts");
        }

        public async Task<Cart> DeleteCart(string id, string user_id,string product_id)
        {
             var filter = Builders<Cart>.Filter.Eq(c => c.UserId,user_id);
            var updateSet = Builders<Cart>.Update.PullFilter(c => c.CartProduct,p => p.ProductId == product_id );
            return await _cart.FindOneAndUpdateAsync(filter, updateSet);
        }

        public async Task<Cart> FindById(string id)
        {
            return await _cart.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public IEnumerable<Cart> GetCart(string user_id)
        {
            var cartList = _cart.Aggregate().Match(Builders<Cart>.Filter.Eq(c => c.UserId, user_id)).Lookup("Products", "CartProduct.ProductId", "_id", "ProductDetails").Project(BsonDocument.Parse(@"{
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
            var cartRes = cartList.Select(doc => BsonSerializer.Deserialize<Cart>(doc));
            return cartRes;
        }

        public async Task<Cart> UpdateQuantity(string user_id, string product_id,int quantity)
        {
            var filter = Builders<Cart>.Filter.And(
                Builders<Cart>.Filter.Eq("UserId",user_id),
                Builders<Cart>.Filter.Eq("CartProduct.ProductId",product_id)
            ) ;
            var update = Builders<Cart>.Update.Inc<int>("CartProduct.$.Quantity", quantity);
            return await _cart.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Cart> { IsUpsert = true });
        }
    }
}