using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back.models
{
    public class MongoDbSetting
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string ProductsCollectionName { get; set; } = null!;
    }
}