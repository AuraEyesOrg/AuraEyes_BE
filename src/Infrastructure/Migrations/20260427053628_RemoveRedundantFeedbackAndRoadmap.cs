using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantFeedbackAndRoadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OphthalmologistFeedback");

            migrationBuilder.DropTable(
                name: "PatientRoadmaps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OphthalmologistFeedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    consultation_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ophthalmologist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OphthalmologistFeedback", x => x.Id);
                    table.CheckConstraint("CK_OphthalmologistFeedback_Rating", "rating >= 1 AND rating <= 5");
                    table.ForeignKey(
                        name: "FK_OphthalmologistFeedback_ConsultationSessions_consultation_s~",
                        column: x => x.consultation_session_id,
                        principalTable: "ConsultationSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OphthalmologistFeedback_Ophthalmologists_ophthalmologist_id",
                        column: x => x.ophthalmologist_id,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OphthalmologistFeedback_Patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientRoadmaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    FollowUpNeeded = table.Column<bool>(type: "boolean", nullable: false),
                    FollowUpTimeframe = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: ""),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LifestyleAdviceJson = table.Column<string>(type: "text", nullable: false),
                    MedicalDiagnosisId = table.Column<Guid>(type: "uuid", nullable: false),
                    NextStepsJson = table.Column<string>(type: "text", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    RawAiResponse = table.Column<string>(type: "text", nullable: true),
                    RiskLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Summary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    WarningSignsJson = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_OphthalmologistFeedback_consultation_session_id",
                table: "OphthalmologistFeedback",
                column: "consultation_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_OphthalmologistFeedback_created_at",
                table: "OphthalmologistFeedback",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_OphthalmologistFeedback_ophthalmologist_id",
                table: "OphthalmologistFeedback",
                column: "ophthalmologist_id");

            migrationBuilder.CreateIndex(
                name: "IX_OphthalmologistFeedback_patient_id_consultation_session_id",
                table: "OphthalmologistFeedback",
                columns: new[] { "patient_id", "consultation_session_id" },
                unique: true);

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
    }
}
