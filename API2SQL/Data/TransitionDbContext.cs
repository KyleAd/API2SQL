using System;
using Microsoft.EntityFrameworkCore;
using API2SQL.Data.Dto;

namespace API2SQL.Data
{
    public class TransitionDbContext : DbContext
    {
        private const string connectionString = @"Connection String";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<AllRequests.Detail> RequestDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllRequests.Detail>().HasKey(r => r.WorkOrderId);
            modelBuilder.Entity<AllRequests.Detail>().Property(b => b.WorkOrderId).ValueGeneratedNever();
        }
    }
}
