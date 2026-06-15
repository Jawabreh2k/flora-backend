using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flora.Cms.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRevisionHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UiConfigRevisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigId = table.Column<int>(type: "int", nullable: false),
                    Revision = table.Column<int>(type: "int", nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SavedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SavedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UiConfigRevisions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UiConfigRevisions_ConfigId_Revision",
                table: "UiConfigRevisions",
                columns: new[] { "ConfigId", "Revision" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UiConfigRevisions");
        }
    }
}
