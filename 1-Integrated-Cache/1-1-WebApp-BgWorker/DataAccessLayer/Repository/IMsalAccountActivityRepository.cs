using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public interface IMsalAccountActivityRepository
    {
        Task<MsalAccountActivity> UpsertActivity(MsalAccountActivity accountActivity);
        Task<IEnumerable<MsalAccountActivity>> GetAccountsToRefresh();
    }
}
