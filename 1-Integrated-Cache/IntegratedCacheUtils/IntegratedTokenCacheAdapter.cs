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

        protected override async Task OnBeforeWriteAsync(TokenCacheNotificationArgs args)
        {
            var accountActivity = new MsalAccountActivity(args.Account);
            await UpsertActivity(accountActivity);

            await Task.FromResult(base.OnBeforeWriteAsync(args));
        }

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

    public static class IntegratedTokenCacheExtensions
    {
        /// <summary>Adds an integrated per-user .NET Core distributed based token cache.</summary>
        /// <param name="services">The services collection to add to.</param>
        /// <returns>A <see cref="IServiceCollection"/> to chain.</returns>
        public static IServiceCollection AddIntegratedUserTokenCache(
            this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSingleton<IMsalTokenCacheProvider, IntegratedTokenCacheAdapter>();
            return services;
        }

        /// <summary>Adds an integrated per-user .NET Core distributed based token cache.</summary>
        /// <param name="builder">The Authentication builder to add to.</param>
        /// <returns>A <see cref="AuthenticationBuilder"/> to chain.</returns>
        public static AuthenticationBuilder AddIntegratedUserTokenCache(
            this AuthenticationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddIntegratedUserTokenCache();
            return builder;
        }
    }
}
