using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLayer.Entities
{
    public class MsalAccountActivity
    {
        public MsalAccountActivity() { }

        public MsalAccountActivity(IAccount account, string cacheKey)
        {
            CacheKey = cacheKey;
            AccountObjectId = account.HomeAccountId.ObjectId;
            AccountIdentifier = account.HomeAccountId.Identifier;
            AccountTenantId = account.HomeAccountId.TenantId;
            LastActivity = DateTime.Now;
        }

        [Key]
        public string CacheKey { get; set; }
        public string AccountObjectId { get; set; }
        public string AccountIdentifier { get; set; }
        public string AccountTenantId { get; set; }
        //public string Username { get; set; }
        //public string Environment { get; set; }
        public DateTime LastActivity { get; set; }
        public bool FailedToRefresh { get; set; }
    }
}
