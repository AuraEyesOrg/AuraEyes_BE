using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiScreeningPatientVisitId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientVisitId",
                table: "AiScreenings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AiScreenings_PatientVisitId",
                table: "AiScreenings",
                column: "PatientVisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_AiScreenings_PatientVisits_PatientVisitId",
                table: "AiScreenings",
                column: "PatientVisitId",
                principalTable: "PatientVisits",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiScreenings_PatientVisits_PatientVisitId",
                table: "AiScreenings");

            migrationBuilder.DropIndex(
                name: "IX_AiScreenings_PatientVisitId",
                table: "AiScreenings");

            migrationBuilder.DropColumn(
                name: "PatientVisitId",
                table: "AiScreenings");
        }
    }
}
