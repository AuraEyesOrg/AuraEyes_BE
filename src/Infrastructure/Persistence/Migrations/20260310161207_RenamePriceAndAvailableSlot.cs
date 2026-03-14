using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamePriceAndAvailableSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Availabilities_AvailabilityId",
                table: "Schedules");

            migrationBuilder.DropTable(
                name: "Availabilities");

            migrationBuilder.DropColumn(
                name: "OfflineClinicFee",
                table: "ConsultationSessions");

            migrationBuilder.RenameColumn(
                name: "AvailabilityId",
                table: "Schedules",
                newName: "AvailableSlotId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_AvailabilityId",
                table: "Schedules",
                newName: "IX_Schedules_AvailableSlotId");

            migrationBuilder.RenameColumn(
                name: "PlatformFee",
                table: "ConsultationSessions",
                newName: "Price");

            migrationBuilder.CreateTable(
                name: "AvailableSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: true),
                    OphthalmologistId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailableSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailableSlots_Ophthalmologists_OphthalmologistId",
                        column: x => x.OphthalmologistId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvailableSlots_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableSlots_OphthalmologistId_StartTime_EndTime",
                table: "AvailableSlots",
                columns: new[] { "OphthalmologistId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableSlots_OrganisationId_StartTime_EndTime",
                table: "AvailableSlots",
                columns: new[] { "OrganisationId", "StartTime", "EndTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_AvailableSlots_AvailableSlotId",
                table: "Schedules",
                column: "AvailableSlotId",
                principalTable: "AvailableSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_AvailableSlots_AvailableSlotId",
                table: "Schedules");

            migrationBuilder.DropTable(
                name: "AvailableSlots");

            migrationBuilder.RenameColumn(
                name: "AvailableSlotId",
                table: "Schedules",
                newName: "AvailabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_AvailableSlotId",
                table: "Schedules",
                newName: "IX_Schedules_AvailabilityId");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ConsultationSessions",
                newName: "PlatformFee");

            migrationBuilder.AddColumn<decimal>(
                name: "OfflineClinicFee",
                table: "ConsultationSessions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Availabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false),
                    OphthalmologistId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Availabilities_Ophthalmologists_OphthalmologistId",
                        column: x => x.OphthalmologistId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Availabilities_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_OphthalmologistId_StartTime_EndTime",
                table: "Availabilities",
                columns: new[] { "OphthalmologistId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_OrganisationId_StartTime_EndTime",
                table: "Availabilities",
                columns: new[] { "OrganisationId", "StartTime", "EndTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Availabilities_AvailabilityId",
                table: "Schedules",
                column: "AvailabilityId",
                principalTable: "Availabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
