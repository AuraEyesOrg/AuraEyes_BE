using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientIdToConsentAndConsentWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "Consents",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""Consents"" c
                SET ""PatientId"" = s.""PatientId""
                FROM ""AiScreenings"" s
                WHERE c.""AiScreeningId"" = s.""Id"";
            ");

            migrationBuilder.AlterColumn<Guid>(
                name: "PatientId",
                table: "Consents",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consents_PatientId",
                table: "Consents",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consents_Patients_PatientId",
                table: "Consents",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consents_Patients_PatientId",
                table: "Consents");

            migrationBuilder.DropIndex(
                name: "IX_Consents_PatientId",
                table: "Consents");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Consents");
        }
    }
}
