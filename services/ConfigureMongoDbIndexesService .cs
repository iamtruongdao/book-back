using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BackEnd.services
{
    public class ConfigureMongoDbIndexesService : IHostedService
    {
        private readonly IMongoClient _client;
        private readonly ILogger<ConfigureMongoDbIndexesService> _logger;

        private readonly MongoDbSetting _setting;
        public ConfigureMongoDbIndexesService(IMongoClient client, ILogger<ConfigureMongoDbIndexesService> logger,MongoDbSetting setting)
        => (_client, _logger,_setting) = (client, logger,setting);
  
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var database = _client.GetDatabase(_setting.DatabaseName);
            var collection = database.GetCollection<Product>("Products");
            _logger.LogInformation("Creating 'At' Index on events");
            var indexKeysDefinition = Builders<Product>.IndexKeys.Text(x => x.ProductName).Text(x => x.ProductDescription);
            var IndexOption = new CreateIndexOptions
            {
                Weights = new BsonDocument {
                    {"ProductName",10},
                    {"ProductDescription",1}
                }
            };
            await collection.Indexes.CreateOneAsync(new CreateIndexModel<Product>(indexKeysDefinition,IndexOption), cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}