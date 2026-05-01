using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNullableSlotTemplateAndPatientDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_AppointmentSlots_TemplateDateTime",
                table: "AppointmentSlots");

            migrationBuilder.AddColumn<DateTime>(
                name: "DiscountExpiryDate",
                table: "Patients",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountForNextBooking",
                table: "Patients",
                type: "numeric(3,2)",
                precision: 3,
                scale: 2,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleTemplateId",
                table: "AppointmentSlots",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "UX_AppointmentSlots_TemplateDateTime",
                table: "AppointmentSlots",
                columns: new[] { "ScheduleTemplateId", "Date", "StartTime", "EndTime", "OphthalId" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"ScheduleTemplateId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_AppointmentSlots_TemplateDateTime",
                table: "AppointmentSlots");

            migrationBuilder.DropColumn(
                name: "DiscountExpiryDate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DiscountForNextBooking",
                table: "Patients");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleTemplateId",
                table: "AppointmentSlots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UX_AppointmentSlots_TemplateDateTime",
                table: "AppointmentSlots",
                columns: new[] { "ScheduleTemplateId", "Date", "StartTime", "EndTime", "OphthalId" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
