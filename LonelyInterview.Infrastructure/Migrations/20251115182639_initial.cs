using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LonelyInterview.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "LonelyInterview");

            migrationBuilder.CreateTable(
                name: "Candidates",
                schema: "LonelyInterview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HrManagers",
                schema: "LonelyInterview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrManagers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidatesInfo",
                schema: "LonelyInterview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    WorkExperience = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatesInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatesInfo_Candidates_Id",
                        column: x => x.Id,
                        principalSchema: "LonelyInterview",
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vacancies",
                schema: "LonelyInterview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    RequiredSkills = table.Column<string>(type: "text", nullable: false),
                    NiceToHaveSkills = table.Column<string>(type: "text", nullable: true),
                    MinYearsOfExperience = table.Column<int>(type: "integer", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: false),
                    SalaryFrom = table.Column<decimal>(type: "numeric", nullable: true),
                    SalaryTo = table.Column<decimal>(type: "numeric", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    EmploymentType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    WorkFormat = table.Column<int>(type: "integer", nullable: false),
                    ResponsibleHrId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HrManagerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacancies_HrManagers_HrManagerId",
                        column: x => x.HrManagerId,
                        principalSchema: "LonelyInterview",
                        principalTable: "HrManagers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vacancies_HrManagers_ResponsibleHrId",
                        column: x => x.ResponsibleHrId,
                        principalSchema: "LonelyInterview",
                        principalTable: "HrManagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resumes",
                schema: "LonelyInterview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GitHubUrl = table.Column<string>(type: "text", nullable: true),
                    ActualSkills = table.Column<string>(type: "text", nullable: true),
                    PassiveSkills = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resumes_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalSchema: "LonelyInterview",
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resumes_Vacancies_Id",
                        column: x => x.Id,
                        principalSchema: "LonelyInterview",
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_CandidateId",
                schema: "LonelyInterview",
                table: "Resumes",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_HrManagerId",
                schema: "LonelyInterview",
                table: "Vacancies",
                column: "HrManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_ResponsibleHrId",
                schema: "LonelyInterview",
                table: "Vacancies",
                column: "ResponsibleHrId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidatesInfo",
                schema: "LonelyInterview");

            migrationBuilder.DropTable(
                name: "Resumes",
                schema: "LonelyInterview");

            migrationBuilder.DropTable(
                name: "Candidates",
                schema: "LonelyInterview");

            migrationBuilder.DropTable(
                name: "Vacancies",
                schema: "LonelyInterview");

            migrationBuilder.DropTable(
                name: "HrManagers",
                schema: "LonelyInterview");
        }
    }
}
