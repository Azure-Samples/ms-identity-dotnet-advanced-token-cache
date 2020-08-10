using IntegratedCacheUtils.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntegratedCacheUtils
{
    // DbContext used to access the entity MsalAccountActivity
    public class IntegratedTokenCacheDbContext : DbContext
    {
        public IntegratedTokenCacheDbContext(DbContextOptions<IntegratedTokenCacheDbContext> options) : base(options)
        {
        }

        public DbSet<MsalAccountActivity> MsalAccountActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}