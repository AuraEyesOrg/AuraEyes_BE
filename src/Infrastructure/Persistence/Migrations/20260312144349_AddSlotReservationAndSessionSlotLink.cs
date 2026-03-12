using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSlotReservationAndSessionSlotLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentSlotId",
                table: "ConsultationSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationExpireAt",
                table: "AppointmentSlots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReservedBy",
                table: "AppointmentSlots",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_AppointmentSlotId",
                table: "ConsultationSessions",
                column: "AppointmentSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_Status_ReservationExpireAt",
                table: "AppointmentSlots",
                columns: new[] { "Status", "ReservationExpireAt" },
                filter: "\"Status\" = 'Reserved'");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationSessions_AppointmentSlots_AppointmentSlotId",
                table: "ConsultationSessions",
                column: "AppointmentSlotId",
                principalTable: "AppointmentSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationSessions_AppointmentSlots_AppointmentSlotId",
                table: "ConsultationSessions");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationSessions_AppointmentSlotId",
                table: "ConsultationSessions");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentSlots_Status_ReservationExpireAt",
                table: "AppointmentSlots");

            migrationBuilder.DropColumn(
                name: "AppointmentSlotId",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "ReservationExpireAt",
                table: "AppointmentSlots");

            migrationBuilder.DropColumn(
                name: "ReservedBy",
                table: "AppointmentSlots");
        }
    }
}
