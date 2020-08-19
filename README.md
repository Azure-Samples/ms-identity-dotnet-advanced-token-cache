---
page_type: sample
languages:
- csharp
products:
- dotnet
- aspnet-core
- azure
- azure-active-directory
- azure-cache-redis
- ms-graph
description: "An ASP.Net Core sample that shows how background apps and services can access the MSAL token cache and continue to act on-behalf of users in their absence."
urlFragment: "ms-identity-dotnet-advanced-token-cache"
---

![.NET Core](https://github.com/Azure-Samples/ms-identity-dotnet-advanced-token-cache/workflows/.NET%20Core/badge.svg)

# Accessing the logged-in user's token cache from background apps, APIs and services


These ASP.Net Core samples show how background apps, APIs and services can access  the [MSAL's access token cache](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/token-cache-serialization) and continue to act on-behalf of users in their absence.

The [MSAL.NET](http://aka.ms/msal-net) and the [Microsoft.Identity.Web](https://aka.ms/microsoft-identity-web) libraries provide various mechanisms for apps that authenticate users with the [Microsoft Identity Platform](http://aka.ms/identityplatform) to cache [Access Tokens](https://aka.ms/access-tokens) for various resources like [Microsoft Graph](https://graph.microsoft.com) to a [distributed token cache implementations like Sql Server or Redis](https://github.com/AzureAD/microsoft-identity-web/wiki/token-cache-serialization).

This tutorial explains how by sharing the same app registration in [Azure Portal](https://portal.azure.com/) (sharing the same client Id), background processes and services can still continue to work on behalf of the user after the user has exited the frontend web app.

Today, most background processes that need to work with user's data without them being present to authenticate or re-authenticate make use of [Application Permissions](https://docs.microsoft.com/graph/auth/auth-concepts#microsoft-graph-permissions) which often require an [admin consent](https://docs.microsoft.com/azure/active-directory/develop/v2-admin-consent). This requires an elevation of privilege and is often an unnecessary hassle as the developer originally did not intend to obtain a permission over what a user consents to their app.

## Contents

| File/folder                 | Description                                |
|-----------------------------|--------------------------------------------|
| `1-Integrated-Cache`        | The various applications used in this sample. |
| `CONTRIBUTING.md`           | Guidelines for contributing to the sample. |
| `LICENSE`                   | The license for the sample.                |

## Setup

### Step 1

Using a command line interface such as VS Code integrated terminal, clone or download this repository:

```console
git clone https://github.com/Azure-Samples/ms-identity-dotnet-advanced-token-cache.git
```

> :warning: Given that the name of the sample is quite long, and so are the names of the referenced NuGet packages, you might want to clone it in a folder close to the root of your hard drive, to avoid file size limitations on Windows.

### Step 2

Navigate to the [Sharing the MSAL token cache between a web app and a background console worker app](./1-Integrated-Cache/1-1-WebApp-BgWorker/README.md) tutorial where we'll learn about how to share the token cache between a Web App that signs-in users and a background console worker process.

or

Navigate to the [Share the MSAL token cache between a web app, its web API and a background console worker app](./1-Integrated-Cache\1-2-WebAPI-BgWorker\README.md") tutorial where we'll learn about how to share the token cache between a Web App that signs-in users, its backed Web API and a background console worker process.

## More information

For more information, visit the following links:

- [Articles about the Microsoft identity platform](http://aka.ms/aaddevv2)
- Learn about [Microsoft.Identity.Web](https://github.com/AzureAD/microsoft-identity-web/wiki)
- Learn how to enable distributed caches in [token cache serialization](https://github.com/AzureAD/microsoft-identity-web/wiki/token-cache-serialization)
- [Use HttpClientFactory to implement resilient HTTP requests](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests) used by the Graph custom service

## Community Help and Support

Use [Stack Overflow](http://stackoverflow.com/questions/tagged/msal) to get support from the community.
Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before.
Make sure that your questions or comments are tagged with [`msal` `dotnet` `azure-active-directory`].

If you find a bug in the sample, please raise the issue on [GitHub Issues](../../issues).

To provide a recommendation, visit the following [User Voice page](https://feedback.azure.com/forums/169401-azure-active-directory).

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments
