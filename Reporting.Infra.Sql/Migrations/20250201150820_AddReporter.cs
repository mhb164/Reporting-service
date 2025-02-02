using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tizpusoft.Reporting.Migrations
{
    /// <inheritdoc />
    public partial class AddReporter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReporterId",
                table: "Details",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Reporter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reporter", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Details_ReporterId",
                table: "Details",
                column: "ReporterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Reporter_ReporterId",
                table: "Details",
                column: "ReporterId",
                principalTable: "Reporter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Details_Reporter_ReporterId",
                table: "Details");

            migrationBuilder.DropTable(
                name: "Reporter");

            migrationBuilder.DropIndex(
                name: "IX_Details_ReporterId",
                table: "Details");

            migrationBuilder.DropColumn(
                name: "ReporterId",
                table: "Details");
        }
    }
}
