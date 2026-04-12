using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnsureFullTimeSystemGeneratedTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ScheduleTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql(
                @"INSERT INTO ""SystemSettings"" (""Key"", ""Description"", ""Value"")
                  VALUES ('FULLTIME_MIN_SLOT_COST', 'Minimum auto-generated slot cost for full-time ophthalmologists', '100000')
                  ON CONFLICT (""Key"") DO UPDATE
                  SET ""Description"" = EXCLUDED.""Description"", ""Value"" = EXCLUDED.""Value"";");

            migrationBuilder.Sql(
                @"INSERT INTO ""SystemSettings"" (""Key"", ""Description"", ""Value"")
                  VALUES ('FULLTIME_MAX_SLOT_COST', 'Maximum auto-generated slot cost for full-time ophthalmologists', '400000')
                  ON CONFLICT (""Key"") DO UPDATE
                  SET ""Description"" = EXCLUDED.""Description"", ""Value"" = EXCLUDED.""Value"";");

            // Keep only one active system-generated template per ophthalmologist/day before adding unique index.
            migrationBuilder.Sql(
                @"WITH ranked AS (
                    SELECT
                        ""Id"",
                        ROW_NUMBER() OVER (
                            PARTITION BY ""OphthalId"", ""DayOfWeek""
                            ORDER BY COALESCE(""UpdatedAt"", ""CreatedAt"") DESC, ""CreatedAt"" DESC, ""Id"" DESC
                        ) AS rn
                    FROM ""ScheduleTemplates""
                    WHERE ""IsDeleted"" = false
                      AND ""IsActive"" = true
                      AND ""Source"" = 'SystemGenerated'
                      AND ""OphthalId"" IS NOT NULL
                )
                UPDATE ""ScheduleTemplates"" st
                SET ""IsActive"" = false,
                    ""UpdatedAt"" = NOW()
                FROM ranked r
                WHERE st.""Id"" = r.""Id""
                  AND r.rn > 1;");

            migrationBuilder.CreateIndex(
                name: "UX_ScheduleTemplates_FullTime_SystemGenerated_Day",
                table: "ScheduleTemplates",
                columns: new[] { "OphthalId", "DayOfWeek" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"IsActive\" = true AND \"Source\" = 'SystemGenerated' AND \"OphthalId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_ScheduleTemplates_FullTime_SystemGenerated_Day",
                table: "ScheduleTemplates");

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Key",
                keyValue: "FULLTIME_MAX_SLOT_COST");

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Key",
                keyValue: "FULLTIME_MIN_SLOT_COST");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ScheduleTemplates");
        }
    }
}
