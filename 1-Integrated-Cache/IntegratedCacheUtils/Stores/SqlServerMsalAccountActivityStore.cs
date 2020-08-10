using IntegratedCacheUtils.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegratedCacheUtils.Stores
{
    // Store used by this sample to illustrate a scenario where you are storying the MsalAccountActivity on a Sql Server database
    public class SqlServerMsalAccountActivityStore : IMsalAccountActivityStore
    {
        private IntegratedTokenCacheDbContext _dbContext;

        public SqlServerMsalAccountActivityStore(IntegratedTokenCacheDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Retrieve MsalAccountActivites that happened before a certain time ago
        public async Task<IEnumerable<MsalAccountActivity>> GetMsalAccountActivitesSince(DateTime lastActivityDate)
        {
            return await _dbContext.MsalAccountActivities
                .Where(x => x.FailedToAcquireToken == false
                    && x.LastActivity <= lastActivityDate)
                .ToListAsync();
        }

        // Retireve a specific user MsalAccountActivity
        public async Task<MsalAccountActivity> GetMsalAccountActivityForUser(string userPrincipalName)
        {
            return await _dbContext.MsalAccountActivities
                            .Where(x => x.FailedToAcquireToken == false
                                && x.UserPrincipalName == userPrincipalName)
                            .FirstOrDefaultAsync();
        }

        // Setting the flag FailedToAcquireToken to true
        public async Task HandleIntegratedTokenAcquisitionFailure(MsalAccountActivity failedAccountActivity)
        {
            failedAccountActivity.FailedToAcquireToken = true;
            _dbContext.MsalAccountActivities.Update(failedAccountActivity);
            await _dbContext.SaveChangesAsync();
        }

        // Insert a new MsalAccountActivity case it doesnt exist, otherwise update the existing entry
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