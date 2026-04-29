using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePatientQuotaFieldsAndCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationSessions_AppointmentSlots_AppointmentSlotId",
                table: "ConsultationSessions");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationSessions_AppointmentSlotId",
                table: "ConsultationSessions");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationSessions_StaleSessionLookup",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "OphthalId",
                table: "ScheduleTemplates");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "ScheduleTemplates");

            migrationBuilder.DropColumn(
                name: "PurchasedAiQuota",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UsedAiQuota",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ActualMonthlySalary",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "CommissionRate",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "ExpectedMonthlySalary",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "WorkingHoursPerWeek",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "AppointmentSlotId",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "CalendarEventId",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "ClosingReason",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "IsAIResultShared",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "IsRetinalImagesShared",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "LastReminderSentAt",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "MeetingLink",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "ReservationExpireAt",
                table: "AppointmentSlots");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "AppointmentSlots");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_StaleSessionLookup",
                table: "ConsultationSessions",
                columns: new[] { "Status", "ChatStatus", "LastActivityAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConsultationSessions_StaleSessionLookup",
                table: "ConsultationSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "OphthalId",
                table: "ScheduleTemplates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ScheduleTemplates",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Doctor");

            migrationBuilder.AddColumn<int>(
                name: "PurchasedAiQuota",
                table: "Patients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsedAiQuota",
                table: "Patients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualMonthlySalary",
                table: "Ophthalmologists",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionRate",
                table: "Ophthalmologists",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedMonthlySalary",
                table: "Ophthalmologists",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Ophthalmologists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Ophthalmologists",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "Ophthalmologists",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "PendingVerification");

            migrationBuilder.AddColumn<int>(
                name: "WorkingHoursPerWeek",
                table: "Ophthalmologists",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "Ophthalmologists",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentSlotId",
                table: "ConsultationSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendarEventId",
                table: "ConsultationSessions",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "ConsultationSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClosedBy",
                table: "ConsultationSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosingReason",
                table: "ConsultationSessions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAIResultShared",
                table: "ConsultationSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRetinalImagesShared",
                table: "ConsultationSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReminderSentAt",
                table: "ConsultationSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeetingLink",
                table: "ConsultationSessions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationExpireAt",
                table: "AppointmentSlots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "AppointmentSlots",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Doctor");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_AppointmentSlotId",
                table: "ConsultationSessions",
                column: "AppointmentSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_StaleSessionLookup",
                table: "ConsultationSessions",
                columns: new[] { "Status", "ChatStatus", "LastActivityAt", "LastReminderSentAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationSessions_AppointmentSlots_AppointmentSlotId",
                table: "ConsultationSessions",
                column: "AppointmentSlotId",
                principalTable: "AppointmentSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
