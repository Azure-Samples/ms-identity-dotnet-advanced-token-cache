﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public class GraphServiceClientFactory
    {
        public static GraphServiceClient GetAuthenticatedGraphClient(Func<Task<string>> acquireAccessToken,
                                                                                 string baseUrl)
        {

            return new GraphServiceClient(baseUrl, new CustomAuthenticationProvider(acquireAccessToken));
        }
    }

    class CustomAuthenticationProvider : IAuthenticationProvider
    {
        public CustomAuthenticationProvider(Func<Task<string>> acquireTokenCallback)
        {
            acquireAccessToken = acquireTokenCallback;
        }

        private readonly Func<Task<string>> acquireAccessToken;

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            string accessToken = await acquireAccessToken.Invoke();

            // Append the access token to the request.
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
        }
    }
}
