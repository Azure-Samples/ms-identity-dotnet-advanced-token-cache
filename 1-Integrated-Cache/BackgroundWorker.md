### BackgroundWorker project

The background worker application, needs to **use the same ApplicationId (ClientId) as the web app (or web api)**, the same `IMsalAccountActivityStore` implementation as the web app (or web api), however it will use `MsalDistributedTokenCacheAdapter` for the token cache provider since we don't need to persist the `MsalAccountActivity.cs` entity in this project.

The main tasks that the background worker application needs to perform in order to use the same token cache as the web app (or web api) are:

- Retrieve `MsalAccountActivity` entities to acquire an access token for
- For each entity retrieved, hydrate a class that extends [`IAccount`](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/blob/master/src/client/Microsoft.Identity.Client/IAccount.cs)
- Call `AcquireTokenSilent()` passing the hydrated [`IAccount`](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/blob/master/src/client/Microsoft.Identity.Client/IAccount.cs) as parameter.

#### Handling MsalUiRequiredException

If MSAL throws a `MsalUiRequiredException` while acquiring a token for an account, it means that an interaction with the user of that account is required. That could be MFA(multi-factor authentication), token expired or revoked and a new login is needed, the cached token got deleted, etc.

Since the background worker doesn't have any user interaction so it could solve this exception, it will have to wait that user to interact with the app and handle the required UI interaction.

This sample is setting the flag `FailedToAcquireToken` to **true** for every account that the background worker couldn't acquire a token, thus it won`t try to acquire it again for those accounts in the next run. You can change this logic to better fit your use case.