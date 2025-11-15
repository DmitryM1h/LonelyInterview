using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LonelyInterview.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidatesInfo_Candidates_Id",
                schema: "LonelyInterview",
                table: "CandidatesInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_CandidatesInfo_Id",
                schema: "LonelyInterview",
                table: "Candidates",
                column: "Id",
                principalSchema: "LonelyInterview",
                principalTable: "CandidatesInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_CandidatesInfo_Id",
                schema: "LonelyInterview",
                table: "Candidates");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesInfo_Candidates_Id",
                schema: "LonelyInterview",
                table: "CandidatesInfo",
                column: "Id",
                principalSchema: "LonelyInterview",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
