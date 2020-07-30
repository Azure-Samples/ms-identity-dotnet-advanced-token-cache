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