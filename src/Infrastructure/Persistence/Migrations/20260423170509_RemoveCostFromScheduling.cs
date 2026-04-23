using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCostFromScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "ScheduleTemplates");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "AppointmentSlots");

            migrationBuilder.AddColumn<decimal>(
                name: "ConsultationFee",
                table: "Ophthalmologists",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Appointments",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PricingType",
                table: "Appointments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestedDoctorId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_RequestedDoctorId",
                table: "Appointments",
                column: "RequestedDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Ophthalmologists_RequestedDoctorId",
                table: "Appointments",
                column: "RequestedDoctorId",
                principalTable: "Ophthalmologists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Ophthalmologists_RequestedDoctorId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_RequestedDoctorId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ConsultationFee",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PricingType",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "RequestedDoctorId",
                table: "Appointments");

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "ScheduleTemplates",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "AppointmentSlots",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }
    }
}
