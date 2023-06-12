using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using GroupeIsa.Neos.Shared.MultiTenant;
using Microsoft.Extensions.Caching.Memory;

namespace Transversals.Business.UserPermissions.Domain.MemoryCache
{
    public class CoreMemoryCache : ICoreMemoryCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly NeosTenantInfo _currentTenant;

        public CoreMemoryCache(IMemoryCache memoryCache, INeosTenantInfoAccessor neosTenantInfoAccessor)
        {
            _memoryCache = memoryCache;
            _currentTenant = neosTenantInfoAccessor.NeosTenantInfo ?? NeosTenantInfo.Empty;
        }

        ///<inheritdoc/>
        public bool TryGetValue<TItem>(string category, string key, [NotNullWhen(true)] out TItem? outputValue)
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

            return (await _memoryCache.GetOrCreateAsync(ComputeFullKey(category, key), (entry) =>
            {
                entry.SetSlidingExpiration(slidingExpiration.Value);
                return factory.Invoke();
            }))!;
        }
        private string ComputeFullKey(string category, string key)
        {
            return $"{_currentTenant.Id}_{category}_{key}";
        }

    }
}
