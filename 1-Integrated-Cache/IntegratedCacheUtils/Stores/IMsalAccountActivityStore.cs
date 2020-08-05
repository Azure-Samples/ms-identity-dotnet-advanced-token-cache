using IntegratedCacheUtils.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedCacheUtils.Stores
{
    // TODO: Comment
    public interface IMsalAccountActivityStore
    {
        Task UpsertMsalAccountActivity(MsalAccountActivity msalAccountActivity);
        Task<IEnumerable<MsalAccountActivity>> GetMsalAccountActivitesSince(DateTime lastActivityDate);
        Task<MsalAccountActivity> GetMsalAccountActivityForUser(string userPrincipalName);
        Task HandleIntegratedTokenAcquisitionFailure(MsalAccountActivity failedAccountActivity);
    }
}
