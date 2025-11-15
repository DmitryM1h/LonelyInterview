using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LonelyInterview.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_Vacancies_Id",
                schema: "LonelyInterview",
                table: "Resumes");

            migrationBuilder.AddColumn<Guid>(
                name: "VacancyId",
                schema: "LonelyInterview",
                table: "Resumes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_VacancyId",
                schema: "LonelyInterview",
                table: "Resumes",
                column: "VacancyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_Vacancies_VacancyId",
                schema: "LonelyInterview",
                table: "Resumes",
                column: "VacancyId",
                principalSchema: "LonelyInterview",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_Vacancies_VacancyId",
                schema: "LonelyInterview",
                table: "Resumes");

            migrationBuilder.DropIndex(
                name: "IX_Resumes_VacancyId",
                schema: "LonelyInterview",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "VacancyId",
                schema: "LonelyInterview",
                table: "Resumes");

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_Vacancies_Id",
                schema: "LonelyInterview",
                table: "Resumes",
                column: "Id",
                principalSchema: "LonelyInterview",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
