using Microsoft.Identity.Client;
using System;
using System.ComponentModel.DataAnnotations;

namespace IntegratedCacheUtils.Entities
{
    // This entity represents the user IAccount that MSAL used to acquire an access token, with additional properties to help the background worker
    // to link a cached token with its correspondent user.
    // Feel free to include more properties that are relevant to your use case
    public class MsalAccountActivity
    {
        public MsalAccountActivity()
        {
        }

        public MsalAccountActivity(string cacheKey, IAccount account)
        {
            AccountCacheKey = cacheKey;
            AccountIdentifier = account.HomeAccountId.Identifier;
            AccountObjectId = account.HomeAccountId.ObjectId;
            AccountTenantId = account.HomeAccountId.TenantId;
            UserPrincipalName = account.Username;
            LastActivity = DateTime.Now;
        }

        [Key]
        public string AccountCacheKey { get; set; }

        public string AccountIdentifier { get; set; }
        public string AccountObjectId { get; set; }
        public string AccountTenantId { get; set; }
        public string UserPrincipalName { get; set; }
        public DateTime LastActivity { get; set; }
        public bool FailedToAcquireToken { get; set; }
    }
}