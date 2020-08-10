using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebAPI
{
    /// <summary>
    /// Class used to add the bearer token header with the access token obtained for the signed user
    /// </summary>
    internal class TokenAcquisitionCredentialProvider : IAuthenticationProvider
    {
        public TokenAcquisitionCredentialProvider(ITokenAcquisition tokenAcquisition, IEnumerable<string> initialScopes)
        {
            _tokenAcquisition = tokenAcquisition;
            _initialScopes = initialScopes;
        }

        ITokenAcquisition _tokenAcquisition;
        IEnumerable<string> _initialScopes;

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            request.Headers.Add("Authorization",
                $"Bearer {await _tokenAcquisition.GetAccessTokenForUserAsync(_initialScopes)}");
        }
    }
}
