using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDailySlotQuotaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailySlotQuotas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_DailySlotQuotas_UpdatedAt",
                table: "DailySlotQuotas",
                column: "UpdatedAt");
        }
    }
}
