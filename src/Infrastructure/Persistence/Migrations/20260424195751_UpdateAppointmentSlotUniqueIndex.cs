using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentSlotUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_AppointmentSlots_TemplateDateTime",
                table: "AppointmentSlots");

            migrationBuilder.CreateIndex(
                name: "UX_AppointmentSlots_TemplateDateTime",
                table: "AppointmentSlots",
                columns: new[] { "ScheduleTemplateId", "Date", "StartTime", "EndTime", "OphthalId" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_AppointmentSlots_TemplateDateTime",
                table: "AppointmentSlots");

            migrationBuilder.CreateIndex(
                name: "UX_AppointmentSlots_TemplateDateTime",
                table: "AppointmentSlots",
                columns: new[] { "ScheduleTemplateId", "Date", "StartTime", "EndTime" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
