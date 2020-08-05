using IntegratedCacheUtils.Entities;
using IntegratedCacheUtils.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.TokenCacheProviders;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using System;
using System.Threading.Tasks;

namespace IntegratedCacheUtils
{
    // An extension of MsalDistributedTokenCacheAdapter, that will upsert the entity MsalAccountActivity
    // before MSAL writes an entry in the token cache
    public class IntegratedTokenCacheAdapter : MsalDistributedTokenCacheAdapter
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public IntegratedTokenCacheAdapter(
            IServiceScopeFactory scopeFactory,
            IDistributedCache memoryCache,
            IOptions<MsalDistributedTokenCacheAdapterOptions> cacheOptions):base(memoryCache, cacheOptions) 
        {
            _scopeFactory = scopeFactory;
        }

        // Overriding OnBeforeWriteAsync to upsert the entity MsalAccountActivity
        // before MSAL writes an entry in the token cache
        protected override async Task OnBeforeWriteAsync(TokenCacheNotificationArgs args)
        {
            var accountActivity = new MsalAccountActivity(args.SuggestedCacheKey, args.Account);
            await UpsertActivity(accountActivity);

            await Task.FromResult(base.OnBeforeWriteAsync(args));
        }

        // Call the upsert method of the class that implements IMsalAccountActivityStore 
        private async Task<MsalAccountActivity> UpsertActivity(MsalAccountActivity accountActivity)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _integratedTokenCacheStore = scope.ServiceProvider.GetRequiredService<IMsalAccountActivityStore>();

                await _integratedTokenCacheStore.UpsertMsalAccountActivity(accountActivity);

                return accountActivity;
            }
        }
    }
}
