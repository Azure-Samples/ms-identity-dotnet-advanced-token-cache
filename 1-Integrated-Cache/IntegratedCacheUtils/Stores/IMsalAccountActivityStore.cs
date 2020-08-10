using IntegratedCacheUtils.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegratedCacheUtils.Stores
{
    // Interface for basics operations of MsalAccountActivity.
    // Implement this interface with your own logic on where to persist the entity, how to handle failures and etc
    public interface IMsalAccountActivityStore
    {
        Task UpsertMsalAccountActivity(MsalAccountActivity msalAccountActivity);

        Task<IEnumerable<MsalAccountActivity>> GetMsalAccountActivitesSince(DateTime lastActivityDate);

        Task<MsalAccountActivity> GetMsalAccountActivityForUser(string userPrincipalName);

        Task HandleIntegratedTokenAcquisitionFailure(MsalAccountActivity failedAccountActivity);
    }
}