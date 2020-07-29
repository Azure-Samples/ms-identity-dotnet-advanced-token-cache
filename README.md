---
page_type: sample
languages:
- csharp
products:
- dotnet
- azure-ad
- ms-graph
- redis
description: "ASP.Net Core samples to integrate a distributed token cache with other applications under the same App Registration"
urlFragment: "ms-identity-dotnet-advanced-token-cache"
---

# ASP.Net Core samples to integrate a distributed token cache with other applications under the same App Registration

This tutorial demonstrates advanced token cache scenarios, such as integrating a distributed token cache with other applications that are under the same App Registration on [Azure Portal](https://portal.azure.com/), sharing the same ClientId.

## Contents

| File/folder                 | Description                                |
|-----------------------------|--------------------------------------------|
| `1-Integrated-Cache`        | Integrating the token cache between applications. |
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

Navigate to [1-Integrated-Cache](./1-Integrated-Cache/1-1-WebApp-BgWorker/README.md) where we'll learn about integrating a distributed token cache from a Web App.

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