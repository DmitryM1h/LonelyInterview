using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LonelyInterview.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_CandidatesInfo_InfoId",
                schema: "LonelyInterview",
                table: "Candidates");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_InfoId",
                schema: "LonelyInterview",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "InfoId",
                schema: "LonelyInterview",
                table: "Candidates");

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

            migrationBuilder.AddColumn<Guid>(
                name: "InfoId",
                schema: "LonelyInterview",
                table: "Candidates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_InfoId",
                schema: "LonelyInterview",
                table: "Candidates",
                column: "InfoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_CandidatesInfo_InfoId",
                schema: "LonelyInterview",
                table: "Candidates",
                column: "InfoId",
                principalSchema: "LonelyInterview",
                principalTable: "CandidatesInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
