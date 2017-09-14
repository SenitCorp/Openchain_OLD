using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OpenchainPlatform.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anchors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AnchorId = table.Column<byte[]>(nullable: true),
                    FullLedgerHash = table.Column<byte[]>(nullable: true),
                    Position = table.Column<byte[]>(nullable: false),
                    TransactionCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anchors", x => x.Id);
                    table.UniqueConstraint("AK_Anchors_Position", x => x.Position);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    Key = table.Column<byte[]>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<byte[]>(nullable: true),
                    Version = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    MutationHash = table.Column<byte[]>(nullable: false),
                    RawData = table.Column<byte[]>(nullable: true),
                    TransactionHash = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.UniqueConstraint("AK_Transactions_MutationHash", x => x.MutationHash);
                    table.UniqueConstraint("AK_Transactions_TransactionHash", x => x.TransactionHash);
                });

            migrationBuilder.CreateTable(
                name: "RecordMutations",
                columns: table => new
                {
                    RecordKey = table.Column<byte[]>(nullable: false),
                    TransactionId = table.Column<long>(nullable: false),
                    MutationHash = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordMutations", x => new { x.RecordKey, x.TransactionId });
                    table.ForeignKey(
                        name: "FK_RecordMutations_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordMutations_TransactionId",
                table: "RecordMutations",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anchors");

            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.DropTable(
                name: "RecordMutations");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
