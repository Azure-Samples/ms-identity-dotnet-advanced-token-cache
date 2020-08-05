using Microsoft.Identity.Client;

namespace BackgroundWorker
{
    // TODO: Comment, very well
    public class MsalAccount : IAccount
    {
        public MsalAccount() { }

        public MsalAccount(string objectId, string tenantId)
        {
            HomeAccountId = new AccountId($"{objectId}.{tenantId}", objectId, tenantId);
        }

        public string Username { get; set; }

        public string Environment { get; set; }

        public AccountId HomeAccountId { get; set; }
    }
}
