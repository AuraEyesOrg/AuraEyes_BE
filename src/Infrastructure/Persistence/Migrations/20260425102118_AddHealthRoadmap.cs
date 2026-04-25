using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthRoadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthRoadmaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthRoadmaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthRoadmaps_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsultationSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MedicalRecordNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AdministrativeDataJson = table.Column<string>(type: "text", nullable: false),
                    ClinicalDataJson = table.Column<string>(type: "text", nullable: false),
                    FinalDiagnosis = table.Column<string>(type: "text", nullable: false),
                    TreatmentPlan = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthRoadmapSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoadmapId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StepType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlannedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedByDoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedFromVisitId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthRoadmapSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthRoadmapSteps_HealthRoadmaps_RoadmapId",
                        column: x => x.RoadmapId,
                        principalTable: "HealthRoadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HealthRoadmapSteps_Ophthalmologists_CreatedByDoctorId",
                        column: x => x.CreatedByDoctorId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HealthRoadmapSteps_PatientVisits_CreatedFromVisitId",
                        column: x => x.CreatedFromVisitId,
                        principalTable: "PatientVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthRoadmaps_PatientId",
                table: "HealthRoadmaps",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HealthRoadmapSteps_CreatedByDoctorId",
                table: "HealthRoadmapSteps",
                column: "CreatedByDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthRoadmapSteps_CreatedFromVisitId",
                table: "HealthRoadmapSteps",
                column: "CreatedFromVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthRoadmapSteps_RoadmapId_PlannedDate",
                table: "HealthRoadmapSteps",
                columns: new[] { "RoadmapId", "PlannedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HealthRoadmapSteps_Status",
                table: "HealthRoadmapSteps",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthRoadmapSteps");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "HealthRoadmaps");
        }
    }
}
