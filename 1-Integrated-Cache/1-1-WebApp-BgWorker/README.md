---
services: active-directory
platforms: dotnet
author: tiagobrenck
level: 400
client: ASP.NET Core Web App
service: Microsoft Graph
endpoint: Microsoft identity platform
---

# Using an integrated token cache between a web app and a background worker

## Scenario

A .NET Core MVC Web app that uses OpenId Connect to sign in users and then calls [Microsoft Graph](https://docs.microsoft.com/graph/overview) `/me` endpoint. It leverages the Microsoft Authentication Library [MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) to acquire an [access token](https://docs.microsoft.com/azure/active-directory/develop/access-tokens) for Graph, and the NuGet package [`Microsoft.Identity.Web`](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki) to configure a distributed token cache.

Then, a .NET Core console application that shares the same ClientId with the Web App, and can acquire a token for [Microsoft Graph](https://docs.microsoft.com/graph/overview) silently, using the access token cached from the Web App. Although this console application doesn't have user interactions, it doesn't not require *Application Permissions* to call [Microsoft Graph](https://docs.microsoft.com/graph/overview).

![Diagram](ReadmeFiles/diagram.png)

## How to run this sample

Pre-requisites:

- If you want to store the token cache on a **SQL Server database**, you can easily generate the token cache table by installing the following tool using the **Developer Command Prompt for Visual Studio** (running as administrator): 
    ```shell
    dotnet tool install --global dotnet-sql-cache
    ```
- If you don't have a SQL Server database to be used in this sample yet, [please create one](https://docs.microsoft.com//sql/relational-databases/databases/create-a-database?view=sql-server-ver15). You can name it as you wish.   

## Step 1: Clone the repository

From your shell or command line:

```console
git clone https://github.com/Azure-Samples/ms-identity-dotnet-advanced-token-cache.git
```

or download and extract the repository .zip file.

> :warning: Given that the name of the sample is quiet long, and so are the names of the referenced packages, you might want to clone it in a folder close to the root of your hard drive, to avoid file size limitations on Windows.

## Step 2: Register the Web App project with your Azure AD tenant

To register the Web App project with your Azure AD tenant, [please follow the step 1 in this doc](https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/2-WebApp-graph-user/2-1-Call-MSGraph#step-1-register-the-sample-with-your-azure-ad-tenant), and name the application as you wish, for instance `IntegratedWebApp-AdvancedToken`.

### Step 2.1: Configure the Web App project to use your app registration

Open the project in your IDE (like Visual Studio) to configure the code.
>In the steps below, "ClientID" is the same as "Application ID" or "AppId".

1. Open the `appsettings.json` file for the `WebApp`.
2. Find the app key `ClientId` and replace the existing value with the application ID (clientId) of the `IntegratedWebApp-AdvancedToken` copied from the Azure portal.
3. Find the app key `TenantId` and replace the value with the Tenant ID where you registered your application.
4. Find the app key `Domain` and replace the existing value with your Azure AD tenant name.
5. Find the app key `ClientSecret` and replace the existing value with the key you saved during the creation of the `IntegratedWebApp-AdvancedToken`, in the Azure portal.
6. Find the section `ConnectionStrings` and replace the value of the keys that are relevant to your scenario:
   - If you will use **SQL Server**, update the key `TokenCacheDbConnStr` with the database connection string.
   - If you will use **Redis**, update the key `TokenCacheRedisConnStr` with the Redis connection string, and the key `TokenCacheRedisInstaceName` with the the Redis instance name.

- In case you want to deploy your app in Sovereign or national clouds, ensure the `GraphApiUrl` option matches the one you want. By default this is Microsoft Graph in the Azure public cloud

  ```JSon
   "GraphApiUrl": "https://graph.microsoft.com/v1.0"
  ```

## Step 3: Register the Background Worker project with your Azure AD tenant

In order to have the Web App and the Background Worker sharing the same token cache, **they must share the same application ID (clientId)** on Azure AD as well. So for this step, you will set additional configuration to the existing `IntegratedWebApp-AdvancedToken` app registration.

1. Navigate to your `IntegratedWebApp-AdvancedToken` [App registration](https://go.microsoft.com/fwlink/?linkid=2083908) page.
2. In the app's registration screen, click on the **Authentication** blade in the left.
3. In the **Authentication** section, click on **Add a platform**:
    - Choose **Mobile and desktop applications**
    - Under **Redirect URIs** select the option, `https://login.microsoftonline.com/common/oauth2/nativeclient`
    - Select **Configure** in the bottom

### Step 3.1: Configure the Background Worker project to use your app registration

Open the project in your IDE (like Visual Studio) to configure the code.
>In the steps below, "ClientID" is the same as "Application ID" or "AppId".

1. Open the `appsettings.json` file for the `BackgroundWorker`.
2. Find the app key `ClientId` and replace the existing value with the application ID (clientId) of the `IntegratedWebApp-AdvancedToken` copied from the Azure portal.
3. Find the app key `TenantId` and replace the value with the Tenant ID where you registered your application.
4. Find the app key `Domain` and replace the existing value with your Azure AD tenant name.
5. Find the app key `ClientSecret` and replace the existing value with the key you saved during the creation of the `IntegratedWebApp-AdvancedToken`, in the Azure portal.
6. Find the section for the `ConnectionStrings` and replace the value of the keys that are relevant to your scenario:
   - If you will use **SQL Server**, update the key `TokenCacheDbConnStr` with the database connection string.
   - If you will use **Redis**, update the key `TokenCacheRedisConnStr` with the Redis connection string, and the key `TokenCacheRedisInstaceName` with the the Redis instance name.

## Step 4: Configure the SQL Server database

>Note: You just need to apply the Entity Framework migrations once, when you are running the sample for the very first time.

This sample uses a SQL Server to store the entity `MsalAccountActivity`, and by using [Entity Framework migrations](https://docs.microsoft.com/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli), you can create the entity table scheme in your database with few steps:

1. On Visual Studio, open the **Package Manager Console** tab.
2. On the **Default Project** dropdown, select `IntegratedCacheUtils`.
3. On the console, execute the command `Update-Database`. 

If your solution is building without errors and you have setup the database connection string, this script will create the table `MsalAccountActivities`. If you are not using Visual Studio, please [check how to apply Entity Framework migrations via CLI](https://docs.microsoft.com/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli).

### Storing the token cache on the SQL Server database

>NOTE: If you are storing the token cache on Redis, you can skip this step.

If you want to store the token cache on your database as well, you must create the the token cache table before. To do so, open the **Developer Command Prompt for Visual Studio** (running as administrator) and run the following script, replacing the connection string value with your own:

```shell
dotnet sql-cache create "Data Source=<Your-DB-connection-string>" dbo <table-name-to-be-created>
```

Example:

```shell
dotnet sql-cache create "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MsalTokenCacheDatabase;Integrated Security=True;" dbo TokenCache
```

## Step 5: Run the sample

To populate the distributed token cache, and the entity `MsalAccountActivity`, the **WebApp must be executed first**. Open the WebApp on multiple browser tabs (you might want to open the tabs in incognito) and sign-in with multiple users. **Do not sign-out, otherwise their token cache will be deleted**.

Once you have signed-in with at least 2 users, stop the WebApp project, **without signing them out** and execute the BackgroundWorker project.

The background worker is returning all account activities that happened more than 5 minutes ago. You could either change the time interval or wait for it. 

With all the accounts retrieved, the background worker will print those that got their token acquired successfully and those that failed. To test a failure scenario, you can sign-out one of the users in the WebApp, and execute the background worker again. 

## About The code

### IntegratedCacheUtils project

The `IntegratedCacheUtils` project contains useful classes to achieve a token cache integration between a web app and a background worker. 

#### MsalAccountActivity Entity

While the web app has a user session that can be used to distinguish who's cached token belongs to whom, the background worker doesn't have this user session concept. To facilitate this link between a user and their cache on the background worker project, we have the entity, `MsalAccountActivity.cs`, that holds enough information to create this link.

#### IntegratedTokenCacheAdapter Extension

The NuGet package, [`Microsoft.Identity.Web`](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki), provides many token cache adapters to be used out of the box, and one of them is the `MsalDistributedTokenCacheAdapter`. This adapter is designed to leverage the .NET distributed token cache library, and we will use it on both web app and background worker project.

For the web app project, however, we would like to extend the `MsalDistributedTokenCacheAdapter` and override the method `OnBeforeWriteAsync()` to hydrate and persist the entity `MsalAccountActivity.cs` so the background worker can use each account to link to its correspondent token cache.

```c#
public class IntegratedTokenCacheAdapter : MsalDistributedTokenCacheAdapter
{
    // removed for brevity...
    protected override async Task OnBeforeWriteAsync(TokenCacheNotificationArgs args)
    {
        var accountActivity = new MsalAccountActivity(args.Account);
        await UpsertActivity(accountActivity);

        await Task.FromResult(base.OnBeforeWriteAsync(args));
    }
    // removed for brevity...
}
```

#### IMsalAccountActivityStore Interface

The `IMsalAccountActivityStore.cs` is an interface to decouple the `IntegratedTokenCacheAdapter.cs` from any specific storage source for the `MsalAccountActivity.cs` entity. Rather it be a SQL Server, MySQL, Redis database, etc, you can provide the persistency logic on the `UpsertActivity()` method and even extend the `MsalAccountActivity.cs` entity to add more properties that could be relevant to your user-case.

### Web App project

In the Web App project, we leverage [`Microsoft.Identity.Web`](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki) to configure the authorization code flow and access token acquisition, and use 
the `IntegratedTokenCacheAdapter.cs` as an extension for the `MsalDistributedTokenCacheAdapter`, so that before MSAL writes a token cache, we hydrate and save the `MsalAccountActivity.cs` entity. 

```c#
services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftWebApp(Configuration)
    .AddMicrosoftWebAppCallsWebApi(Configuration, new string[] { Constants.ScopeUserRead })
    .AddIntegratedUserTokenCache();
```

To store the distributed token cache, you could use SQL Server or Redis for instance:

SQL Server, leveraging `Microsoft.Extensions.Caching.Sql` NuGet package:

```c#
services.AddDistributedSqlServerCache(options =>
{
    /*
        To create the token cache table:
        dotnet tool install --global dotnet-sql-cache
        dotnet sql-cache create "<YourDatabaseConnectionString>" dbo <TokenCacheTableName>    
    */
    options.ConnectionString = Configuration.GetConnectionString("TokenCacheDbConnStr");
    options.SchemaName = "dbo";
    options.TableName = "<TokenCacheTableName>";
});
```

Redis, leveraging `Microsoft.Extensions.Caching.StackExchangeRedis` NuGet package:

```c#
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Configuration.GetConnectionString("TokenCacheRedisConnStr");
    options.InstanceName = Configuration.GetConnectionString("TokenCacheRedisInstaceName");
});
```
And setup the Dependency Injection for the desired `IMsalAccountActivityStore` implementation. For instance:

Save `MsalAccountActivity.cs` entity on SQL Server: 
```c#
services.AddDbContext<IntegratedTokenCacheDbContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("TokenCacheDbConnStr")));
services.AddScoped<IIntegratedTokenCacheStore, IntegratedSqlServerTokenCacheStore>();
```

### BackgroundWorker project

The background worker application, needs to **use the same ApplicationId (ClientId) as the web app**, the same `IMsalAccountActivityStore` implementation as the web app, however it will use `MsalDistributedTokenCacheAdapter` for the token cache provider since we don't need to persist the `MsalAccountActivity.cs` entity in this project.

The main tasks that the background worker application needs to perform in order to use the same token cache as the web app are:

- Retrieve `MsalAccountActivity` entities to acquire an access token for
- For each entity retrieved, hydrate a class that extends [`IAccount`](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/blob/master/src/client/Microsoft.Identity.Client/IAccount.cs)
- Call `AcquireTokenSilent()` passing the hydrated [`IAccount`](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/blob/master/src/client/Microsoft.Identity.Client/IAccount.cs) as parameter.

#### Handling MsalUiRequiredException

If MSAL throws a `MsalUiRequiredException` while acquiring a token for an account, it means that an interaction with the user of that account is required. That could be MFA(multi-factor authentication), token expired or revoked and a new login is needed, the cached token got deleted, etc.

Since the background worker doesn't have any user interaction so it could solve this exception, it will have to wait that user to access the web app and handle the required UI interaction. In this sample, we are setting the flag `FailedToAcquireToken` to **true**, but it is up to you to implement a logic that goes with your user-case.

## Learn more

- Learn how [Microsoft.Identity.Web](../../Microsoft.Identity.Web) works, in particular hooks-up to the ASP.NET Core ODIC events
- Learn how to enable distributed caches in [token cache serialization](../2-2-TokenCache)
- [Use HttpClientFactory to implement resilient HTTP requests](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests) used by the Graph custom service
