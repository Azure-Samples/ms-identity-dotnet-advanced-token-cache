using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class MsalAccountActivityRepository : IMsalAccountActivityRepository
    {
        private readonly CacheDbContext _dbContext;

        public MsalAccountActivityRepository(CacheDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<MsalAccountActivity>> GetAccountsToRefresh()
        {
            DateTime thresholdDate = DateTime.Now.AddMinutes(-15);
            return await _dbContext.MsalAccountActivities
                .Where(x => x.FailedToRefresh == false)
                        //&& x.LastActivity <= thresholdDate)
                .ToListAsync();
        }

        public async Task<MsalAccountActivity> UpsertActivity(MsalAccountActivity accountActivity)
        {
            // TODO: Implement concurrency logic

            if (_dbContext.MsalAccountActivities.Count(x => x.CacheKey == accountActivity.CacheKey) != 0)
                _dbContext.Update(accountActivity);
            else
                _dbContext.MsalAccountActivities.Add(accountActivity);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return accountActivity;
        }
    }
}