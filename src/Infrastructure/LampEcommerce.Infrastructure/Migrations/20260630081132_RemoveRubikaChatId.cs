using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LampEcommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRubikaChatId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RubikaChatId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RubikaChatId",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
