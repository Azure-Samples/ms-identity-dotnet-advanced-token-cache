using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundWorker
{
    public class BackgroundWorkerTokenCacheAdapter : MsalDistributedTokenCacheAdapter
    {
        /// <summary>
        /// .NET Core Memory cache.
        /// </summary>
        private readonly IDistributedCache _distributedCache;

        /// <summary>
        /// MSAL memory token cache options.
        /// </summary>
        private readonly MsalDistributedTokenCacheAdapterOptions _cacheOptions;

        private readonly string _cacheKey;

        public BackgroundWorkerTokenCacheAdapter(string cacheKey, 
            IDistributedCache distributedCache, 
            IOptions<MsalDistributedTokenCacheAdapterOptions> cacheOptions) 
            : base(distributedCache, cacheOptions)
        {
            _cacheKey = cacheKey;
            _distributedCache = distributedCache;
            _cacheOptions = cacheOptions?.Value;
        }

        protected override async Task RemoveKeyAsync(string cacheKey)
        {
            await _distributedCache.RemoveAsync(_cacheKey).ConfigureAwait(false);
        }

        protected override async Task<byte[]> ReadCacheBytesAsync(string cacheKey)
        {
            return await _distributedCache.GetAsync(_cacheKey).ConfigureAwait(false);
        }

        protected override async Task WriteCacheBytesAsync(string cacheKey, byte[] bytes)
        {
            await _distributedCache.SetAsync(_cacheKey, bytes, _cacheOptions).ConfigureAwait(false);
        }
    }
}
