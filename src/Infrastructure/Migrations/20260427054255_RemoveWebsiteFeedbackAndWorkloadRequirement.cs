using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWebsiteFeedbackAndWorkloadRequirement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebsiteFeedback");

            migrationBuilder.DropTable(
                name: "WorkloadRequirements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebsiteFeedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteFeedback", x => x.Id);
                    table.CheckConstraint("CK_WebsiteFeedback_Rating", "rating >= 1 AND rating <= 5");
                    table.ForeignKey(
                        name: "FK_WebsiteFeedback_Patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkloadRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    EmploymentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PeriodType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequiredHours = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkloadRequirements", x => x.Id);
                    table.CheckConstraint("CK_WorkloadRequirements_RequiredHours_NonNegative", "\"RequiredHours\" >= 0");
                });

            migrationBuilder.InsertData(
                table: "WorkloadRequirements",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EmploymentType", "PeriodType", "RequiredHours", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("4387bc41-31be-4d0e-a5a4-30bd95c79112"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "FullTime", "Week", 40m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system" },
                    { new Guid("bc5004a0-a6ce-484a-a2e5-8af7559f95fb"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "FullTime", "Month", 160m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteFeedback_created_at",
                table: "WebsiteFeedback",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteFeedback_patient_id",
                table: "WebsiteFeedback",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "UX_WorkloadRequirements_Employment_Period",
                table: "WorkloadRequirements",
                columns: new[] { "EmploymentType", "PeriodType" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
