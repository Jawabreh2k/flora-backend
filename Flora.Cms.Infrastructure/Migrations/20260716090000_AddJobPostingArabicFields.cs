using Flora.Cms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flora.Cms.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(FloraCmsDbContext))]
    [Migration("20260716090000_AddJobPostingArabicFields")]
    public partial class AddJobPostingArabicFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "JobPostings",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequirementsAr",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "ResponsibilitiesAr",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "TitleAr", table: "JobPostings");
            migrationBuilder.DropColumn(name: "DescriptionAr", table: "JobPostings");
            migrationBuilder.DropColumn(name: "RequirementsAr", table: "JobPostings");
            migrationBuilder.DropColumn(name: "ResponsibilitiesAr", table: "JobPostings");
        }
    }
}
