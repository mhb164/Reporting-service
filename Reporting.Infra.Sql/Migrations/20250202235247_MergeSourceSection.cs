using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tizpusoft.Reporting.Migrations
{
    /// <inheritdoc />
    public partial class MergeSourceSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Details_Sections_SectionId",
                table: "Details");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Details",
                newName: "SourceSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Details_SectionId",
                table: "Details",
                newName: "IX_Details_SourceSectionId");

            migrationBuilder.CreateTable(
                name: "SourceSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceSections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SourceSections_Source_Section",
                table: "SourceSections",
                columns: new[] { "Source", "Section" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Details_SourceSections_SourceSectionId",
                table: "Details",
                column: "SourceSectionId",
                principalTable: "SourceSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Details_SourceSections_SourceSectionId",
                table: "Details");

            migrationBuilder.DropTable(
                name: "SourceSections");

            migrationBuilder.RenameColumn(
                name: "SourceSectionId",
                table: "Details",
                newName: "SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Details_SourceSectionId",
                table: "Details",
                newName: "IX_Details_SectionId");

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_Name",
                table: "Sections",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sections_SourceId",
                table: "Sections",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Name",
                table: "Sources",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Sections_SectionId",
                table: "Details",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
