using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LonelyInterview.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_HrManagers_HrManagerId",
                schema: "LonelyInterview",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_HrManagerId",
                schema: "LonelyInterview",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "HrManagerId",
                schema: "LonelyInterview",
                table: "Vacancies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HrManagerId",
                schema: "LonelyInterview",
                table: "Vacancies",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_HrManagerId",
                schema: "LonelyInterview",
                table: "Vacancies",
                column: "HrManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_HrManagers_HrManagerId",
                schema: "LonelyInterview",
                table: "Vacancies",
                column: "HrManagerId",
                principalSchema: "LonelyInterview",
                principalTable: "HrManagers",
                principalColumn: "Id");
        }
    }
}
