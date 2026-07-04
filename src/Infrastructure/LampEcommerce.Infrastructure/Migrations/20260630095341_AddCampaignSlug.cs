using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LampEcommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Campaigns",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Slug",
                table: "Campaigns",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Campaigns_Slug",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Campaigns");
        }
    }
}
