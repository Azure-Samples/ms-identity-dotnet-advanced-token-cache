using IntegratedCacheUtils.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedCacheUtils.Stores
{
    public class SqlServerMsalAccountActivityStore : IMsalAccountActivityStore
    {
        private IntegratedTokenCacheDbContext _dbContext;

        public SqlServerMsalAccountActivityStore(IntegratedTokenCacheDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<MsalAccountActivity>> GetMsalAccountActivitesSince(DateTime lastActivityDate)
        {
            return await _dbContext.MsalAccountActivities
                .Where(x => x.FailedToAcquireToken == false 
                    && x.LastActivity <= lastActivityDate)
                .ToListAsync();
        }

        public async Task<MsalAccountActivity> GetMsalAccountActivityForUser(string userPrincipalName)
        {
            return await _dbContext.MsalAccountActivities
                            .Where(x => x.FailedToAcquireToken == false
                                && x.UserPrincipalName == userPrincipalName)
                            .FirstOrDefaultAsync();
        }

        public async Task HandleIntegratedTokenAcquisitionFailure(MsalAccountActivity failedAccountActivity)
        {
            failedAccountActivity.FailedToAcquireToken = true;
            _dbContext.MsalAccountActivities.Update(failedAccountActivity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpsertMsalAccountActivity(MsalAccountActivity msalAccountActivity)
        {
            if (_dbContext.MsalAccountActivities.Count(x => x.AccountCacheKey == msalAccountActivity.AccountCacheKey) != 0)
                _dbContext.Update(msalAccountActivity);
            else
                _dbContext.MsalAccountActivities.Add(msalAccountActivity);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
