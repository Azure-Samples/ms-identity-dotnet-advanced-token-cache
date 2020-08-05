using IntegratedCacheUtils.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegratedCacheUtils
{
    // TODO: Comment
    public class IntegratedTokenCacheDbContext : DbContext
    {
        public IntegratedTokenCacheDbContext(DbContextOptions<IntegratedTokenCacheDbContext> options) : base(options) { }

        public DbSet<MsalAccountActivity> MsalAccountActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
