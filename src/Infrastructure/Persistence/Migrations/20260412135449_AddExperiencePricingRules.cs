using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExperiencePricingRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExperiencePricingRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MinYearsExperience = table.Column<int>(type: "integer", nullable: false),
                    MaxYearsExperience = table.Column<int>(type: "integer", nullable: false),
                    MinPrice = table.Column<decimal>(type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    MaxPrice = table.Column<decimal>(type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperiencePricingRules", x => x.Id);
                    table.CheckConstraint("CK_ExperiencePricingRules_IntegerPrice", "\"MinPrice\" = TRUNC(\"MinPrice\") AND \"MaxPrice\" = TRUNC(\"MaxPrice\")");
                    table.CheckConstraint("CK_ExperiencePricingRules_PriceRange", "\"MinPrice\" > 0 AND \"MaxPrice\" >= \"MinPrice\"");
                    table.CheckConstraint("CK_ExperiencePricingRules_YearsRange", "\"MinYearsExperience\" >= 0 AND \"MaxYearsExperience\" >= \"MinYearsExperience\"");
                });

            migrationBuilder.InsertData(
                table: "ExperiencePricingRules",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "MaxPrice", "MaxYearsExperience", "MinPrice", "MinYearsExperience", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("1b7d3f39-e7b7-46e0-80ef-c5436a95f7af"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", true, 300000m, 5, 200000m, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system" },
                    { new Guid("65f0a3f8-17f0-4bc4-a97a-d5f11f63a2d1"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", true, 200000m, 2, 100000m, 0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system" },
                    { new Guid("70eb8d16-3709-4d06-a3d2-8f65f5f31184"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", true, 400000m, 70, 300000m, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system" }
                });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Key",
                keyValue: "FULLTIME_SLOT_WINDOW_DAYS",
                column: "Value",
                value: "7");

            migrationBuilder.CreateIndex(
                name: "UX_ExperiencePricingRules_YearsBand",
                table: "ExperiencePricingRules",
                columns: new[] { "MinYearsExperience", "MaxYearsExperience" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExperiencePricingRules");

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Key",
                keyValue: "FULLTIME_SLOT_WINDOW_DAYS",
                column: "Value",
                value: "30");
        }
    }
}
