using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Transversals.Business.Core.Domain.MemoryCache
{
    public interface ICoreMemoryCache
    {
        /// <summary>
        /// Gets the or create asynchronous.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        /// <returns></returns>
        Task<TItem> GetOrCreateAsync<TItem>(string category, string key, Func<Task<TItem>> factory, TimeSpan? slidingExpiration = null);

        /// <summary>
        /// Removes the specified category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        void Remove(string category, string key);

        /// <summary>
        /// Sets the specified category.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        TItem Set<TItem>(string category, string key, TItem value, MemoryCacheEntryOptions options);

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <param name="outputValue">The output value.</param>
        /// <returns></returns>
        bool TryGetValue<TItem>(string category, string key, out TItem outputValue);
    }
}