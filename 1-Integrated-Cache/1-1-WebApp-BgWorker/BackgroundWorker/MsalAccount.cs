using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackgroundWorker
{
    public class MsalAccount : IAccount
    {
        public MsalAccount() { }

        public MsalAccount(string userName, string environment, string objectId, string tenantId)
        {
            //Username = userName;
            //Environment = environment;
            HomeAccountId = new AccountId($"{objectId}.{tenantId}", objectId, tenantId);
        }

        public string Username { get; set; }

        public string Environment { get; set; }

        public AccountId HomeAccountId { get; set; }
    }
}
