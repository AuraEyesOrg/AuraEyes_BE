using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLastReminderSentAtToConsultationSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConsultationSessions_StaleSessionLookup",
                table: "ConsultationSessions");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReminderSentAt",
                table: "ConsultationSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_StaleSessionLookup",
                table: "ConsultationSessions",
                columns: new[] { "Status", "ChatStatus", "LastActivityAt", "LastReminderSentAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConsultationSessions_StaleSessionLookup",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "LastReminderSentAt",
                table: "ConsultationSessions");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_StaleSessionLookup",
                table: "ConsultationSessions",
                columns: new[] { "Status", "ChatStatus", "LastActivityAt" });
        }
    }
}
