using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDailySlotQuotaAndSlotSources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ScheduleTemplates",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Doctor");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "AppointmentSlots",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Doctor");

                        migrationBuilder.Sql(@"
                                UPDATE ""ScheduleTemplates"" st
                                SET ""Source"" = 'SystemGenerated'
                                FROM ""Ophthalmologists"" o
                                WHERE st.""OphthalId"" = o.""Id""
                                    AND o.""EmploymentType"" = 'FullTime';
                        ");

                        migrationBuilder.Sql(@"
                                UPDATE ""AppointmentSlots"" s
                                SET ""Source"" = 'System'
                                FROM ""ScheduleTemplates"" st
                                WHERE s.""ScheduleTemplateId"" = st.""Id""
                                    AND st.""Source"" = 'SystemGenerated';
                        ");

            migrationBuilder.CreateTable(
                name: "DailySlotQuotas",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    PartTimeSlotCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    QuotaSnapshot = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySlotQuotas", x => x.Date);
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Key", "Description", "Value" },
                values: new object[,]
                {
                    { "FULLTIME_SLOT_WINDOW_DAYS", "Rolling window (days) for auto-generating full-time slots", "30" },
                    { "PART_TIME_MAX_SLOTS_PER_DAY", "Global daily slot quota for all part-time ophthalmologists", "100" }
                });

            migrationBuilder.CreateIndex(
                name: "UX_AppointmentSlots_TemplateDateTime",
                table: "AppointmentSlots",
                columns: new[] { "ScheduleTemplateId", "Date", "StartTime", "EndTime" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_DailySlotQuotas_UpdatedAt",
                table: "DailySlotQuotas",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailySlotQuotas");

            migrationBuilder.DropIndex(
                name: "UX_AppointmentSlots_TemplateDateTime",
                table: "AppointmentSlots");

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Key",
                keyValue: "FULLTIME_SLOT_WINDOW_DAYS");

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Key",
                keyValue: "PART_TIME_MAX_SLOTS_PER_DAY");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "ScheduleTemplates");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "AppointmentSlots");
        }
    }
}
