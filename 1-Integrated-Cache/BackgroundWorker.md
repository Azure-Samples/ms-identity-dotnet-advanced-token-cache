# The console Background worker project

The background worker project is a classic example of an application's background process that continues to work irrespective of whether a user is signed in or not.

Since access tokens for resources like Microsoft Graph or Azure Services will expire after some time, the background processes can no longer call these services on user's behalf.

So background worker application, uses the **same ApplicationId (ClientId) as the web app (or web api)**, and shares the same `IMsalAccountActivityStore` implementation as the web app (or web api). However it will use `MsalDistributedTokenCacheAdapter` for the token cache provider since we don't need to persist the `MsalAccountActivity.cs` entity in this project.

By sharing the app id with the front-end web app, the console app is able to then use MSAL to retrieve the cached token.

The main tasks that the background worker application needs to perform in order to use the same token cache as the web app (or web api) are:

- Retrieve `MsalAccountActivity` entities to acquire an access token for
- For each entity retrieved, hydrate a class that extends [`IAccount`](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/blob/master/src/client/Microsoft.Identity.Client/IAccount.cs)
- Call `AcquireTokenSilent()` passing the hydrated [`IAccount`](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/blob/master/src/client/Microsoft.Identity.Client/IAccount.cs) as parameter.

## Handling MsalUiRequiredException

If MSAL throws a `MsalUiRequiredException` while acquiring a token for an account, it means that an interaction with the user of that account is required. That could be MFA(multi-factor authentication), token expired or revoked and a new login is needed, the cached token got deleted, etc.

Since the background worker doesn't have any user interaction so it could solve this exception, it will have to wait that user to interact with the app and handle the required UI interaction.

This sample is setting the flag `FailedToAcquireToken` to **true** for every account that the background worker couldn't acquire a token, thus it won`t try to acquire it again for those accounts in the next run. You can change this logic to better fit your use case.