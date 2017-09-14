using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Openchain.EntityFrameworkCore;
using Openchain.Infrastructure;

namespace OpenchainPlatform.Server.Migrations
{
    [DbContext(typeof(OpenchainDbContext))]
    partial class OpenchainDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Openchain.EntityFrameworkCore.Models.Anchor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("AnchorId");

                    b.Property<byte[]>("FullLedgerHash");

                    b.Property<byte[]>("Position")
                        .IsRequired();

                    b.Property<long>("TransactionCount");

                    b.HasKey("Id");

                    b.HasAlternateKey("Position");

                    b.ToTable("Anchors");
                });

            modelBuilder.Entity("Openchain.EntityFrameworkCore.Models.Record", b =>
                {
                    b.Property<byte[]>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Type");

                    b.Property<byte[]>("Value");

                    b.Property<byte[]>("Version");

                    b.HasKey("Key");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("Openchain.EntityFrameworkCore.Models.RecordMutation", b =>
                {
                    b.Property<byte[]>("RecordKey");

                    b.Property<long>("TransactionId");

                    b.Property<byte[]>("MutationHash");

                    b.HasKey("RecordKey", "TransactionId");

                    b.HasIndex("TransactionId");

                    b.ToTable("RecordMutations");
                });

            modelBuilder.Entity("Openchain.EntityFrameworkCore.Models.Transaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("MutationHash")
                        .IsRequired();

                    b.Property<byte[]>("RawData");

                    b.Property<byte[]>("TransactionHash")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("MutationHash");


                    b.HasAlternateKey("TransactionHash");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Openchain.EntityFrameworkCore.Models.RecordMutation", b =>
                {
                    b.HasOne("Openchain.EntityFrameworkCore.Models.Transaction", "Transaction")
                        .WithMany("RecordMutations")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
