using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenchainPlatform.Core.Data.Models;

namespace OpenchainPlatform.Core.EntityFrameworkCore
{
    public class OpenchainPlatformDbContext : DbContext
    {
        public DbSet<Asset> Assets { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<AccountAddress> AccountAddresses { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransactionEntry> TransactionEntries { get; set; }

        public OpenchainPlatformDbContext(DbContextOptions options) : base(options) { }

        protected OpenchainPlatformDbContext() { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Asset>(b =>
            {
                b.HasKey(e => e.Id);

                b.HasMany(e => e.Accounts).WithOne(e => e.Asset).HasForeignKey(e => e.AssetId).IsRequired();
            });

            builder.Entity<Account>(b =>
            {
                b.HasKey(e => e.Id);

                b.HasOne(e => e.Asset).WithMany(e => e.Accounts).HasForeignKey(e => e.AssetId).IsRequired();

                b.HasMany(e => e.Addresses).WithOne(e => e.Account).HasForeignKey(e => e.AccountId).IsRequired();
            });

            builder.Entity<AccountAddress>(b =>
            {
                b.HasKey(e => e.Address);

                b.HasOne(e => e.Account).WithMany(e => e.Addresses).HasForeignKey(e => e.AccountId).IsRequired();
            });

            builder.Entity<Transaction>(b =>
            {
                b.HasKey(e => e.Id);

                b.HasMany(e => e.Entries).WithOne(e => e.Transaction).HasForeignKey(e => e.TransactionId);
            });

            builder.Entity<TransactionEntry>(b =>
            {
                b.HasKey(e => e.Id);

                b.HasOne(e => e.Transaction).WithMany(e => e.Entries).HasForeignKey(e => e.TransactionId);
            });
        }
    }
}
