using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LonelyInterview.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HrManagerNewFieldAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Company",
                schema: "LonelyInterview",
                table: "HrManagers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                schema: "LonelyInterview",
                table: "HrManagers");
        }
    }
}
