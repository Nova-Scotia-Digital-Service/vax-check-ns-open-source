using Newtonsoft.Json;
using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfVaccine.Token.Providers
{
    public class JwksCache : IJwksCache
    {
        [JsonProperty("sets", Required = Required.Always)]
        public Dictionary<Uri, JwksCacheItem> JsonWebKeySetDictionaryCache;

        [JsonProperty("max_cache_life", Required = Required.Always)]
        public TimeSpan MaxCacheLife;

        public JwksCache(TimeSpan? MaxCacheLife)
        {
            JsonWebKeySetDictionaryCache = new Dictionary<Uri, JwksCacheItem>();
            this.MaxCacheLife = MaxCacheLife ?? new TimeSpan(0, 10, 0);
        }

        [JsonConstructor]
        public JwksCache(Dictionary<Uri, JwksCacheItem> JsonWebKeySetDictionaryCache, TimeSpan MaxCacheLife)
        {
            this.JsonWebKeySetDictionaryCache = JsonWebKeySetDictionaryCache;
            this.MaxCacheLife = MaxCacheLife;
        }

        public void Set(Uri WellKnownUrl, JsonWebKeySet JsonWebKeySet)
        {
            var Item = new JwksCacheItem(WellKnownUrl, JsonWebKeySet, DateTime.Now); ;
            if (JsonWebKeySetDictionaryCache.ContainsKey(WellKnownUrl))
            {
                JsonWebKeySetDictionaryCache[WellKnownUrl] = Item;
            }
            else
            {
                JsonWebKeySetDictionaryCache.Add(WellKnownUrl, Item);
            }
        }
        public Result<JsonWebKeySet> Get(Uri WellKnownUrl)
        {
            //Check the JsonWebKeySet cache for the target
            DateTime now = DateTime.Now;
            if (JsonWebKeySetDictionaryCache.ContainsKey(WellKnownUrl))
            {
                JwksCacheItem JwksCacheItem = JsonWebKeySetDictionaryCache[WellKnownUrl];
                if (DateTime.Now.Subtract(JwksCacheItem.ObtainedDate) > MaxCacheLife)
                {
                    //Expire this JsonWebKeySet and go get it again 
                    JsonWebKeySetDictionaryCache.Remove(WellKnownUrl);
                    return Result<JsonWebKeySet>.Fail("Cache value stale");
                }
                else
                {
                    return Result<JsonWebKeySet>.Ok(JwksCacheItem.JsonWebKeySet);
                }
            }
            return Result<JsonWebKeySet>.Fail("Not found in cache");
        }

        public IList<Result<JsonWebKeySet>> ClearStale(IList<Uri> WellKnownUrls)
        {
            IList<Result<JsonWebKeySet>> results = new List<Result<JsonWebKeySet>>();
            var nonWhitelistedUris = JsonWebKeySetDictionaryCache.Keys.Except(WellKnownUrls).ToList();
            foreach (Uri nonWhitelistUri in nonWhitelistedUris)
            {
                results.Add(Result<JsonWebKeySet>.Ok(JsonWebKeySetDictionaryCache[nonWhitelistUri].JsonWebKeySet));
                JsonWebKeySetDictionaryCache.Remove(nonWhitelistUri);
            }
            return results;
        }
    }
}
