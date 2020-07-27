using IntegratedCacheUtils;
using IntegratedCacheUtils.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.TokenCacheProviders;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundWorker
{
    class Program
    {
        private static ServiceProvider _serviceProvider = null;
        private static IIntegratedTokenCacheStore _integratedTokenCacheStore = null;
        private static AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Application started! \n");

                // SQL SERVER CONFIG
                _serviceProvider = new ServiceCollection()
                    .AddLogging()
                    .AddDistributedMemoryCache()
                    .AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = config.TokenCacheDbConnStr;
                        options.SchemaName = "dbo";
                        options.TableName = "TokenCache";
                    })
                    .AddDbContext<IntegratedTokenCacheDbContext>(options => options.UseSqlServer(config.TokenCacheDbConnStr))
                    .AddSingleton<IMsalTokenCacheProvider, MsalDistributedTokenCacheAdapter>()
                    .AddScoped<IIntegratedTokenCacheStore, IntegratedSqlServerTokenCacheStore>()
                    .BuildServiceProvider();

                // REDIS CONFIG
                //_serviceProvider = new ServiceCollection()
                //    .AddLogging()
                //    .AddDistributedMemoryCache()
                //    .AddStackExchangeRedisCache(options =>
                //    {
                //        options.Configuration = config.TokenCacheRedisConnStr;
                //        options.InstanceName = config.TokenCacheRedisInstaceName;
                //    })
                //    .AddSingleton<IMsalTokenCacheProvider, MsalDistributedTokenCacheAdapter>()
                //    .AddScoped<IIntegratedTokenCacheStore>(x =>
                //        new IntegratedRedisTokenCacheStore(config.TokenCacheRedisConnStr))
                //    .BuildServiceProvider();

                _integratedTokenCacheStore = _serviceProvider.GetRequiredService<IIntegratedTokenCacheStore>();

                while (true)
                {
                    RunAsync().GetAwaiter().GetResult();
                    Thread.Sleep(TimeSpan.FromMinutes(10));
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static async Task RunAsync()
        {
            var scopes = new string[] { "User.Read" };

            // Returns the MsalAccountActivities that you would like to acquire a token silently
            var accountsToAcquireToken = await _integratedTokenCacheStore.GetAllAccounts();

            if (accountsToAcquireToken != null)
            {
                Console.WriteLine($"Trying to acquire token silently for activities...");

                IConfidentialClientApplication app = await GetConfidentialClientApplication(config);

                // For each record, hydrate an IAccount with the values saved on the table, and call AcquireTokenSilent for this account.
                foreach (var account in accountsToAcquireToken)
                {
                    var hydratedAccount = new MsalAccount
                    {
                        HomeAccountId = new AccountId(
                            account.AccountIdentifier,
                            account.AccountObjectId,
                            account.AccountTenantId)
                    };

                    try
                    {
                        var result = await app.AcquireTokenSilent(scopes, hydratedAccount)
                            .ExecuteAsync()
                            .ConfigureAwait(false);

                        Console.WriteLine($"Token silently acquired for account: {account.AccountIdentifier}");
                    }
                    catch (MsalUiRequiredException ex)
                    {
                        /* 
                         * If MsalUiRequiredException is thrown for an account, it means that a user interaction is required 
                         * thus the background worker wont be able to acquire a token silently for it.
                         * The user of that account will have to access the web app to perform this interaction.
                         * Examples that could cause this: MFA requirement, token expired or revoked, token cache deleted, etc
                         */
                        await _integratedTokenCacheStore.HandleIntegratedTokenAcquisitionFailure(account);

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Could not acquire token for account {account.AccountIdentifier}.");
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }   

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Environment.NewLine);
            Console.WriteLine($"Task completed.");
            Console.ResetColor();
        }

        private static async Task<IConfidentialClientApplication> GetConfidentialClientApplication(AuthenticationConfig config)
        {
            var app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(new Uri(config.Authority))
                .Build();

            var msalCache = _serviceProvider.GetService<IMsalTokenCacheProvider>();

            await msalCache.InitializeAsync(app.UserTokenCache);

            return app;
        }
    }
}
