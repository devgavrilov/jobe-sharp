using Microsoft.EntityFrameworkCore.Migrations;

namespace JobeSharp.Migrations
{
    public partial class IndexOnCreationDateTimeOfTheRunForDeletionTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CreationDateTimeUtc",
                table: "Runs",
                column: "CreationDateTimeUtc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CreationDateTimeUtc",
                table: "Runs");
        }
    }
}
