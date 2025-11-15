using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LonelyInterview.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resumes_VacancyId",
                schema: "LonelyInterview",
                table: "Resumes");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_VacancyId",
                schema: "LonelyInterview",
                table: "Resumes",
                column: "VacancyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resumes_VacancyId",
                schema: "LonelyInterview",
                table: "Resumes");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_VacancyId",
                schema: "LonelyInterview",
                table: "Resumes",
                column: "VacancyId",
                unique: true);
        }
    }
}
