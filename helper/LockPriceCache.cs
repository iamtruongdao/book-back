using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace BackEnd.helper
{
    public class LockPriceCache
    {
        private readonly IMemoryCache _cache;
        public LockPriceCache( IMemoryCache cache)
        {
            _cache = cache;
     
        }
        public decimal GetPriceCache(string user_id, string product_id)
        {
            var cacheKey = $"cart_price_{user_id}";
            var userPriceDict = _cache.Get<Dictionary<string, decimal>>(cacheKey)
                                ?? new Dictionary<string, decimal>();
            return userPriceDict[product_id];
        }
    }
}