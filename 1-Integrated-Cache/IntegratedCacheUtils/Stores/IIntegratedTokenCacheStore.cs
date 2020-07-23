using IntegratedCacheUtils.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedCacheUtils.Stores
{
    public interface IIntegratedTokenCacheStore
    {
        Task UpsertMsalAccountActivity(MsalAccountActivity msalAccountActivity);
        Task<IEnumerable<MsalAccountActivity>> GetAllAccounts();
        Task HandleIntegratedTokenAcquisitionFailure(MsalAccountActivity failedAccountActivity);
    }
}
