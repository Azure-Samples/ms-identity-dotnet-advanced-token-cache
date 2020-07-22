using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using Microsoft.Identity.Web.UI;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CacheDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TokenCacheDbConnStr")));
            services.AddScoped<IMsalAccountActivityRepository, MsalAccountActivityRepository>();

            services.AddSingleton<IMsalTokenCacheProvider, IntegratedTokenCacheAdapter>();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
                options.HandleSameSiteCookieCompatibility();
            });

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftWebApp(Configuration)
                .AddMicrosoftWebAppCallsWebApi(Configuration, new string[] { Constants.ScopeUserRead });

                //.AddSignIn(options =>
                //{
                //    Configuration.Bind("AzureAD", options);
                //    //options.Events.OnAuthorizationCodeReceived = async context =>
                //    //{
                //    //    var tokenAcquisition = context.HttpContext.RequestServices.GetRequiredService<ITokenAcquisition>();
                //    //    var cacheProvider = context.HttpContext.RequestServices.GetRequiredService<IMsalTokenCacheProvider>();

                //    //    var app = ConfidentialClientApplicationBuilder.Create(options.ClientId)
                //    //                .WithAuthority(options.Authority)
                //    //                .WithClientSecret(options.ClientSecret)
                //    //                .Build();

                //    //    await cacheProvider.InitializeAsync(app.UserTokenCache);

                //    //    var account = (await app.GetAccountAsync(context.HttpContext.User.GetMsalAccountId()));

                //    //    var accountActivity = new MsalAccountActivity(account, context.HttpContext.User.GetMsalAccountId());

                //    //    var repo = context.HttpContext.RequestServices.GetRequiredService<IMsalAccountActivityRepository>();
                //    //    await repo.UpsertActivity(accountActivity);
                //    //};
                //}, options => { Configuration.Bind("AzureAD", options); });

            // Token acquisition service based on MSAL.NET
            // and chosen token cache implementation
            //services.AddWebAppCallsProtectedWebApi(Configuration, new string[] { Constants.ScopeUserRead });
                //.AddDistributedTokenCaches();

            services.AddDistributedSqlServerCache(options =>
            {
                /*
                    dotnet tool install --global dotnet-sql-cache
                    dotnet sql-cache create "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MY_TOKEN_CACHE_DATABASE;Integrated Security=True;" dbo TokenCache    
                */
                options.ConnectionString = Configuration.GetConnectionString("TokenCacheDbConnStr");
                options.SchemaName = "dbo";
                options.TableName = "TokenCache";
            });

            

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
