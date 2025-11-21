using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LonelyInterview.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class denormalization3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidatesInfo",
                schema: "LonelyInterview");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Info_Degree",
                schema: "LonelyInterview",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "Info_GraduationYear",
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

            migrationBuilder.CreateTable(
                name: "CandidatesInfo",
                schema: "LonelyInterview",
                columns: table => new
                {
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Degree = table.Column<string>(type: "text", nullable: true),
                    GraduationYear = table.Column<int>(type: "integer", nullable: true),
                    Specialty = table.Column<string>(type: "text", nullable: true),
                    WorkExperience = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatesInfo", x => x.CandidateId);
                    table.ForeignKey(
                        name: "FK_CandidatesInfo_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalSchema: "LonelyInterview",
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
