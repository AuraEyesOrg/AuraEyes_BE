using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExpandMedicalDiagnosisSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiagnosesCode",
                table: "MedicalDiagnoses",
                newName: "DiagnosisCode");

            migrationBuilder.RenameColumn(
                name: "DiagnosesText",
                table: "MedicalDiagnoses",
                newName: "ClinicalFindings");

            migrationBuilder.AddColumn<string>(
                name: "CodingSystem",
                table: "MedicalDiagnoses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfidenceLevel",
                table: "MedicalDiagnoses",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalizedAt",
                table: "MedicalDiagnoses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUrgent",
                table: "MedicalDiagnoses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Recommendations",
                table: "MedicalDiagnoses",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeverityLevel",
                table: "MedicalDiagnoses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MedicalDiagnoses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodingSystem",
                table: "MedicalDiagnoses");

            migrationBuilder.DropColumn(
                name: "ConfidenceLevel",
                table: "MedicalDiagnoses");

            migrationBuilder.DropColumn(
                name: "FinalizedAt",
                table: "MedicalDiagnoses");

            migrationBuilder.DropColumn(
                name: "IsUrgent",
                table: "MedicalDiagnoses");

            migrationBuilder.DropColumn(
                name: "Recommendations",
                table: "MedicalDiagnoses");

            migrationBuilder.DropColumn(
                name: "SeverityLevel",
                table: "MedicalDiagnoses");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MedicalDiagnoses");

            migrationBuilder.RenameColumn(
                name: "DiagnosisCode",
                table: "MedicalDiagnoses",
                newName: "DiagnosesCode");

            migrationBuilder.RenameColumn(
                name: "ClinicalFindings",
                table: "MedicalDiagnoses",
                newName: "DiagnosesText");
        }
    }
}
