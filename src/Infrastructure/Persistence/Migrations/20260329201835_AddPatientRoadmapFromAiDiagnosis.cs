using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientRoadmapFromAiDiagnosis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientRoadmaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalDiagnosisId = table.Column<Guid>(type: "uuid", nullable: false),
                    RiskLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Summary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    NextStepsJson = table.Column<string>(type: "text", nullable: false),
                    LifestyleAdviceJson = table.Column<string>(type: "text", nullable: false),
                    WarningSignsJson = table.Column<string>(type: "text", nullable: false),
                    FollowUpNeeded = table.Column<bool>(type: "boolean", nullable: false),
                    FollowUpTimeframe = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: ""),
                    RawAiResponse = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientRoadmaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientRoadmaps_MedicalDiagnoses_MedicalDiagnosisId",
                        column: x => x.MedicalDiagnosisId,
                        principalTable: "MedicalDiagnoses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientRoadmaps_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientRoadmaps_GeneratedAt",
                table: "PatientRoadmaps",
                column: "GeneratedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRoadmaps_MedicalDiagnosisId",
                table: "PatientRoadmaps",
                column: "MedicalDiagnosisId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientRoadmaps_PatientId",
                table: "PatientRoadmaps",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientRoadmaps");
        }
    }
}
