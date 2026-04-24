using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnershipAndCostToScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "ScheduleTemplates",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OphthalId",
                table: "ScheduleTemplates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrgId",
                table: "ScheduleTemplates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "AppointmentSlots",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OphthalId",
                table: "AppointmentSlots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationExpireAt",
                table: "AppointmentSlots",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "ScheduleTemplates");

            migrationBuilder.DropColumn(
                name: "OphthalId",
                table: "ScheduleTemplates");

            migrationBuilder.DropColumn(
                name: "OrgId",
                table: "ScheduleTemplates");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "AppointmentSlots");

            migrationBuilder.DropColumn(
                name: "OphthalId",
                table: "AppointmentSlots");

            migrationBuilder.DropColumn(
                name: "ReservationExpireAt",
                table: "AppointmentSlots");
        }
    }
}
