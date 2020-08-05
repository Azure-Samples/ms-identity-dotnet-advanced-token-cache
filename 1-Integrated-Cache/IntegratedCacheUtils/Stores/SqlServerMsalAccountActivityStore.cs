using IntegratedCacheUtils.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedCacheUtils.Stores
{
    // TODO: Comment
    public class SqlServerMsalAccountActivityStore : IMsalAccountActivityStore
    {
        private IntegratedTokenCacheDbContext _dbContext;

        public SqlServerMsalAccountActivityStore(IntegratedTokenCacheDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // TODO: Comment
        public async Task<IEnumerable<MsalAccountActivity>> GetMsalAccountActivitesSince(DateTime lastActivityDate)
        {
            return await _dbContext.MsalAccountActivities
                .Where(x => x.FailedToAcquireToken == false 
                    && x.LastActivity <= lastActivityDate)
                .ToListAsync();
        }

        // TODO: Comment
        public async Task<MsalAccountActivity> GetMsalAccountActivityForUser(string userPrincipalName)
        {
            return await _dbContext.MsalAccountActivities
                            .Where(x => x.FailedToAcquireToken == false
                                && x.UserPrincipalName == userPrincipalName)
                            .FirstOrDefaultAsync();
        }

        // TODO: Comment
        public async Task HandleIntegratedTokenAcquisitionFailure(MsalAccountActivity failedAccountActivity)
        {
            failedAccountActivity.FailedToAcquireToken = true;
            _dbContext.MsalAccountActivities.Update(failedAccountActivity);
            await _dbContext.SaveChangesAsync();
        }

        // TODO: Comment
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
