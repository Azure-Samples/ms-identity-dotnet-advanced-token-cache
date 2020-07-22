using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer
{
    public class CacheDbContext : DbContext
    {
        public CacheDbContext(DbContextOptions<CacheDbContext> options) : base(options) { }

        public DbSet<MsalAccountActivity> MsalAccountActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
