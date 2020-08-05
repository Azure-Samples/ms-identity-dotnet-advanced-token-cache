using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IntegratedCacheUtils.Entities
{
    // TODO: Comment, very well
    public class MsalAccountActivity
    {
        public MsalAccountActivity() { }

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
