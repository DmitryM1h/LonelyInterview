using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LonelyInterview.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class denormalization2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidatesInfo",
                schema: "LonelyInterview",
                table: "CandidatesInfo");

            migrationBuilder.DropIndex(
                name: "IX_CandidatesInfo_CandidateId",
                schema: "LonelyInterview",
                table: "CandidatesInfo");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "LonelyInterview",
                table: "CandidatesInfo");

            migrationBuilder.DropColumn(
                name: "Info_Degree",
                schema: "LonelyInterview",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "Info_GraduationYear",
                schema: "LonelyInterview",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "Info_Id",
                schema: "LonelyInterview",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "Info_Specialty",
                schema: "LonelyInterview",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "Info_WorkExperience",
                schema: "LonelyInterview",
                table: "Candidates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidatesInfo",
                schema: "LonelyInterview",
                table: "CandidatesInfo",
                column: "CandidateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidatesInfo",
                schema: "LonelyInterview",
                table: "CandidatesInfo");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "LonelyInterview",
                table: "CandidatesInfo",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Info_Degree",
                schema: "LonelyInterview",
                table: "Candidates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Info_GraduationYear",
                schema: "LonelyInterview",
                table: "Candidates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Info_Id",
                schema: "LonelyInterview",
                table: "Candidates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Info_Specialty",
                schema: "LonelyInterview",
                table: "Candidates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info_WorkExperience",
                schema: "LonelyInterview",
                table: "Candidates",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidatesInfo",
                schema: "LonelyInterview",
                table: "CandidatesInfo",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatesInfo_CandidateId",
                schema: "LonelyInterview",
                table: "CandidatesInfo",
                column: "CandidateId");
        }
    }
}
