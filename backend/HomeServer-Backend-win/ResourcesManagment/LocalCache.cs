using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend.ResourcesManagment
{
    internal class LocalCache : IDisposable
    {
        private class CacheData
        {
        public const double default_TTL_Seconds = 3;
            public CacheData(object data, DateTime? expressionTime = null)
            {
                this.ExpressionTime = expressionTime ?? DateTime.Now.AddSeconds(default_TTL_Seconds);
                this.Data = data;
            }

            public object Data;
            public DateTime ExpressionTime;

        }

        private Dictionary<int, CacheData> ResourceCache = new();

        private string owner_name = "";
        public LocalCache(string owner_name = "Unknown")
        {
            this.owner_name = owner_name;
        }

        public void Dispose()
        {
            ResourceCache.Clear();
        }


        /// <summary>
        /// Check the cache for the specified type and return the cached data if it exists and is not expired.
        /// </summary>
        /// <param name="type">int representing The Type of cache to get </param>
        /// <returns> The cache value </returns>
        public object? CheckCache(int type)
        {
            if (ResourceCache.TryGetValue(type, out CacheData? cacheData))
            {
                if (cacheData.ExpressionTime > DateTime.Now)
                {
                    Logger.LogDebug($"[] Getting data from cache: " + type.ToString() + " - " + cacheData.ExpressionTime.ToString("yyyy-MM-dd HH:mm:ss") + " - " + cacheData.Data.ToString());
                    return cacheData.Data;
                }
                else
                {
                    ResourceCache.Remove(type);
                }
            }
            Logger.LogDebug($"[] Cache miss for type: " + type.ToString() + ". Data is either expired or not found.");
            return null;
        }

        /// <summary>
        /// load data into the cache with an optional expiration time.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="expressionTime"></param>
        public void LoadIntoCache(int type, object data, DateTime? expressionTime = null)
        {
            Logger.LogDebug($"Cache Loaded with {type.ToString()} - {data.ToString()} - Expiration Time: {(expressionTime.HasValue ? expressionTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "No expiration")}");
            ResourceCache[type] = new CacheData(data, expressionTime);
        }
    }
}
