using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenchainPlatform.Core.EntityFrameworkCore;

namespace OpenchainPlatform.API.Migrations
{
    [DbContext(typeof(OpenchainPlatformDbContext))]
    [Migration("20170628172857_AddAccountAddressToAsset")]
    partial class AddAccountAddressToAsset
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("OpenchainPlatform.Core.Data.Models.Account", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Alias");

                    b.Property<string>("AssetId")
                        .IsRequired();

                    b.Property<long>("Balance");

                    b.Property<string>("Chain");

                    b.Property<string>("Label");

                    b.Property<string>("Path");

                    b.Property<string>("Seed");

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("OpenchainPlatform.Core.Data.Models.AccountAddress", b =>
                {
                    b.Property<string>("Address")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountId")
                        .IsRequired();

                    b.Property<string>("Chain");

                    b.Property<uint>("Index");

                    b.HasKey("Address");

                    b.HasIndex("AccountId");

                    b.ToTable("AccountAddresses");
                });

            modelBuilder.Entity("OpenchainPlatform.Core.Data.Models.Asset", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountAddress");

                    b.Property<string>("Chain");

                    b.Property<int>("Index");

                    b.Property<long>("IssuedUnits");

                    b.Property<string>("Label");

                    b.Property<string>("Path");

                    b.HasKey("Id");

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("OpenchainPlatform.Core.Data.Models.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Metadata");

                    b.Property<string>("SequenceId");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("OpenchainPlatform.Core.Data.Models.TransactionEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LedgerPath");

                    b.Property<string>("TransactionId");

                    b.Property<long>("Value");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("TransactionEntries");
                });

            modelBuilder.Entity("OpenchainPlatform.Core.Data.Models.Account", b =>
                {
                    b.HasOne("OpenchainPlatform.Core.Data.Models.Asset", "Asset")
                        .WithMany("Accounts")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OpenchainPlatform.Core.Data.Models.AccountAddress", b =>
                {
                    b.HasOne("OpenchainPlatform.Core.Data.Models.Account", "Account")
                        .WithMany("Addresses")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OpenchainPlatform.Core.Data.Models.TransactionEntry", b =>
                {
                    b.HasOne("OpenchainPlatform.Core.Data.Models.Transaction", "Transaction")
                        .WithMany("Entries")
                        .HasForeignKey("TransactionId");
                });
        }
    }
}
