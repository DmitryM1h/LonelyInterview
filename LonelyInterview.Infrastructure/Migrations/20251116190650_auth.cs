using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LonelyInterview.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class auth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Biography",
                schema: "LonelyInterview",
                table: "CandidatesInfo",
                newName: "Specialty");

            migrationBuilder.AddColumn<string>(
                name: "Degree",
                schema: "LonelyInterview",
                table: "CandidatesInfo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GraduationYear",
                schema: "LonelyInterview",
                table: "CandidatesInfo",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Degree",
                schema: "LonelyInterview",
                table: "CandidatesInfo");

            migrationBuilder.DropColumn(
                name: "GraduationYear",
                schema: "LonelyInterview",
                table: "CandidatesInfo");

            migrationBuilder.RenameColumn(
                name: "Specialty",
                schema: "LonelyInterview",
                table: "CandidatesInfo",
                newName: "Biography");
        }
    }
}
