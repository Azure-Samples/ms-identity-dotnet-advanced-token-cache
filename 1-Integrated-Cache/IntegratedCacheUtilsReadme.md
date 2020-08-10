# The IntegratedCacheUtils library

The `IntegratedCacheUtils` project contains the classes that are needed to achieve the token cache sharing between a web app and its background worker.

## MsalAccountActivity Entity

While the web app has a user session that can be used to distinguish who's cached token belongs to whom, the background worker doesn't have this facility available to it. So to overcome this limitation,  and facilitate this link between a user and their cache on the background worker project, we have the entity, `MsalAccountActivity.cs`, a representation of a Sql  table, that holds the necessary information to link a signed-in users account with their cached tokens.

## IntegratedTokenCacheAdapter Extension

The NuGet package, [Microsoft.Identity.Web](https://aka.ms/microsoft-identity-web), provides multiple token cache adapters that can be used out of the box, and one of them is the `MsalDistributedTokenCacheAdapter`. This adapter is designed to leverage the .NET distributed token cache library, and we will use it on both web app and background worker project.

For the web app project, however, we extend the `MsalDistributedTokenCacheAdapter` and override the method `OnBeforeWriteAsync()` to hydrate and persist the `MsalAccountActivity.cs` entity to make this data available to the background worker.

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

## IMsalAccountActivityStore Interface

The `IMsalAccountActivityStore.cs` is an interface to decouple the `IntegratedTokenCacheAdapter.cs` from any specific storage source for the `MsalAccountActivity.cs` entity. Rather it be a SQL Server, MySQL, Redis database, etc, you can provide the persistency logic on the `UpsertActivity()` method and even extend the `MsalAccountActivity.cs` entity to add more properties that could be relevant to your user-case.