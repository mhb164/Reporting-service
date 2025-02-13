using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tizpusoft.Reporting.Migrations
{
    /// <inheritdoc />
    public partial class FixDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Details_Reporter_ReporterId",
                table: "Details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reporter",
                table: "Reporter");

            migrationBuilder.RenameTable(
                name: "Reporter",
                newName: "Reporters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reporters",
                table: "Reporters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Reporters_ReporterId",
                table: "Details",
                column: "ReporterId",
                principalTable: "Reporters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Details_Reporters_ReporterId",
                table: "Details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reporters",
                table: "Reporters");

            migrationBuilder.RenameTable(
                name: "Reporters",
                newName: "Reporter");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reporter",
                table: "Reporter",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Reporter_ReporterId",
                table: "Details",
                column: "ReporterId",
                principalTable: "Reporter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
