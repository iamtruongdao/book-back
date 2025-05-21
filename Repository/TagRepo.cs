using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BackEnd.Viewmodel;
using BackEnd.DTOs.Tag;
using BackEnd.models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BackEnd.Repository
{
    public interface ITagRepository
{
    Task<PaginatedList<Tags>> GetAllAsync( int pageSize, int pageNumber);
    Task<Tags?> GetByIdAsync(string id);
    Task<TagDto> GetBySlugAsync(string slug,int pageSize,int pageNumber);
    Task<Tags> CreateAsync(Tags tag);
    Task<ReplaceOneResult> UpdateAsync(Tags tag);
    Task<DeleteResult> DeleteAsync(string id);
}
    public class TagRepo: ITagRepository
    {   
            private readonly IMongoCollection<Tags> _tagCollection;
        public TagRepo(IMongoClient client,MongoDbSetting dbSetting)
        {
            var database = client.GetDatabase(dbSetting.DatabaseName);
            _tagCollection = database.GetCollection<Tags>("Tags");
        }


        public async Task<PaginatedList<Tags>> GetAllAsync( int pageSize, int pageNumber)
        {
            var filter = Builders<Tags>.Filter.Empty;
            var total = await _tagCollection.CountDocumentsAsync(filter);
            var tags = await _tagCollection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            return new PaginatedList<Tags>(tags, (int)total, pageNumber, pageSize);
        }
        

        public async Task<Tags?> GetByIdAsync(string id) =>
            await _tagCollection.Find(t => t.Id == id).FirstOrDefaultAsync();

        public async Task<TagDto> GetBySlugAsync(string slug,int pageSize,int pageNumber)
        {
            var filter = Builders<Tags>.Filter.Eq(x => x.Slug, slug);
           var countLookup = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Posts" },
                { "let", new BsonDocument("tagId", "$_id") },
                { "pipeline", new BsonArray
                    {
                        new BsonDocument("$match", new BsonDocument
                        {
                            { "$expr", new BsonDocument("$eq", new BsonArray { "$TagId", "$$tagId" }) }
                        })
                    }
                },
                { "as", "AllPosts" }
            });

            // Lookup paged post list
            var pagedLookup = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Posts" },
                { "let", new BsonDocument("tagId", "$_id") },
                { "pipeline", new BsonArray
                    {
                        new BsonDocument("$match", new BsonDocument
                        {
                            { "$expr", new BsonDocument("$eq", new BsonArray { "$TagId", "$$tagId" }) }
                        }),
                        new BsonDocument("$skip", (pageNumber-1)*pageSize),
                        new BsonDocument("$limit", pageSize)
                    }
                },
                { "as", "Tag_Post" }
            });

            // Add count
            var addFields = new BsonDocument("$addFields", new BsonDocument
            {
                { "TotalPage", new BsonDocument("$ceil", new BsonDocument(
                "$divide", new BsonArray { new BsonDocument("$size", "$AllPosts"), pageSize }
            )) }
            });
            var project = new BsonDocument("$project", new BsonDocument
            {
                { "AllPosts", 0 } // ẩn đi nếu không muốn trả về
            });
            return  await _tagCollection.Aggregate()
                .Match(filter)
                .AppendStage<BsonDocument>(countLookup)
                .AppendStage<BsonDocument>(pagedLookup)
                .AppendStage<BsonDocument>(addFields)
                .AppendStage<TagDto>(project)
                .FirstOrDefaultAsync();
            
        }

        public async Task<Tags> CreateAsync(Tags tag)
        {
            await _tagCollection.InsertOneAsync(tag);
            return tag;
         }
            

        public async Task<ReplaceOneResult> UpdateAsync(Tags tag) =>
            await _tagCollection.ReplaceOneAsync(t => t.Id == tag.Id, tag);

        public async Task<DeleteResult> DeleteAsync(string id) =>
            await _tagCollection.DeleteOneAsync(t => t.Id == id);
    }
}