using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flora.Cms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactInquiryStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing inquiries predate triage — treat them as unread (NEW) rather than
            // an invalid empty status.
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ContactInquiries",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "NEW");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInquiries_Status",
                table: "ContactInquiries",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContactInquiries_Status",
                table: "ContactInquiries");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ContactInquiries");
        }
    }
}
