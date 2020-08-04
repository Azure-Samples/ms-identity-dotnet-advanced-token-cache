using IntegratedCacheUtils;
using IntegratedCacheUtils.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.TokenCacheProviders;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundWorker
{
    class Program
    {
        private static ServiceProvider _serviceProvider = null;
        private static IMsalAccountActivityStore _msalAccountActivityStore = null;
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
                    .AddScoped<IMsalAccountActivityStore, SqlServerMsalAccountActivityStore>()
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
                //    .AddDbContext<IntegratedTokenCacheDbContext>(options => options.UseSqlServer(config.TokenCacheDbConnStr))
                //    .AddScoped<IMsalAccountActivityStore, SqlServerMsalAccountActivityStore>()
                //    .BuildServiceProvider();

                _msalAccountActivityStore = _serviceProvider.GetRequiredService<IMsalAccountActivityStore>();

                RunAsync().GetAwaiter().GetResult();
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

            // Return the MsalAccountActivities that you would like to acquire a token silently
            var someTimeAgo = DateTime.Now.AddSeconds(-30);
            var accountsToAcquireToken = await _msalAccountActivityStore.GetMsalAccountActivitesSince(someTimeAgo);

            // Or you could also return the account activity of a particular user
            //var userActivityAccount = await _msalAccountActivityStore.GetMsalAccountActivityForUser("User-UPN");

            if(accountsToAcquireToken == null || accountsToAcquireToken.Count() == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"No accounts returned");
                Console.ResetColor();
            }

            else
            {
                Console.WriteLine($"Trying to acquire token silently for {accountsToAcquireToken.Count()} activities...");
                Console.Write(Environment.NewLine);

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
                        
                        Console.WriteLine($"Token acquired for account: {account.UserPrincipalName}");
                        Console.WriteLine($"Access token preview: {result.AccessToken.Substring(0, 70)} ...");
                        Console.WriteLine("  <------------------------>  ");
                        Console.Write(Environment.NewLine);
                    }
                    catch (MsalUiRequiredException ex)
                    {
                        /* 
                         * If MsalUiRequiredException is thrown for an account, it means that a user interaction is required 
                         * thus the background worker wont be able to acquire a token silently for it.
                         * The user of that account will have to access the web app to perform this interaction.
                         * Examples that could cause this: MFA requirement, token expired or revoked, token cache deleted, etc
                         */
                        await _msalAccountActivityStore.HandleIntegratedTokenAcquisitionFailure(account);

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
