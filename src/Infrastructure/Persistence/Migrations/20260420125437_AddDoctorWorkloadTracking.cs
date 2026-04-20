using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorWorkloadTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "ConsultationSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "ConsultationSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkloadRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmploymentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PeriodType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequiredHours = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
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
                name: "IX_ConsultationSessions_WorkloadLookup",
                table: "ConsultationSessions",
                columns: new[] { "OphthalmologistId", "Status", "StartTime", "EndTime" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ConsultationSessions_EndTime_After_StartTime",
                table: "ConsultationSessions",
                sql: "\"StartTime\" IS NULL OR \"EndTime\" IS NULL OR \"EndTime\" >= \"StartTime\"");

            migrationBuilder.CreateIndex(
                name: "UX_WorkloadRequirements_Employment_Period",
                table: "WorkloadRequirements",
                columns: new[] { "EmploymentType", "PeriodType" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkloadRequirements");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationSessions_WorkloadLookup",
                table: "ConsultationSessions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ConsultationSessions_EndTime_After_StartTime",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ConsultationSessions");
        }
    }
}
