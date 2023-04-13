using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Transversals.Business.Core.Application.Tenants;

namespace Transversals.Business.Core.Domain.MemoryCache
{
    public class CoreMemoryCache : ICoreMemoryCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ITenantAccessor? _tenantAccessor;

        public CoreMemoryCache(IMemoryCache memoryCache, ITenantAccessor? tenantAccessor)
        {
            _memoryCache = memoryCache;
            _tenantAccessor = tenantAccessor;
        }

        ///<inheritdoc/>
        public bool TryGetValue<TItem>(string category, string key, out TItem outputValue)
        {
            return _memoryCache.TryGetValue(ComputeFullKey(category, key), out outputValue);

        }

        ///<inheritdoc/>
        public TItem Set<TItem>(string category, string key, TItem value, MemoryCacheEntryOptions options)
        {
            return _memoryCache.Set(ComputeFullKey(category, key), value, options);
        }

        ///<inheritdoc/>
        public void Remove(string category, string key)
        {
            _memoryCache.Remove(ComputeFullKey(category, key));
        }

        ///<inheritdoc/>
        public async Task<TItem> GetOrCreateAsync<TItem>(string category, string key, Func<Task<TItem>> factory, TimeSpan? slidingExpiration = null)
        {
            if (!slidingExpiration.HasValue)
                slidingExpiration = TimeSpan.FromMinutes(5);

            return await _memoryCache.GetOrCreateAsync(ComputeFullKey(category, key), (entry) =>
            {
                entry.SetSlidingExpiration(slidingExpiration.Value);
                return factory.Invoke();
            });
        }
        private string ComputeFullKey(string category, string key)
        {
            string fullKey = string.Empty;
            if (_tenantAccessor?.TenantsInfo != null)
            {
                fullKey = $"{_tenantAccessor.TenantsInfo.Id}_{category}_{key}";
            }
            else
            {
                fullKey = $"{category}_{key}";
            }
            return fullKey ;
        }

    }
}
