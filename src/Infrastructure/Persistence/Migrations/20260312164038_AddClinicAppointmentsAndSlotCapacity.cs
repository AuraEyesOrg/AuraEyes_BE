using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicAppointmentsAndSlotCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxCapacity",
                table: "AppointmentSlots",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "ClinicAppointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedDoctorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VisitReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CheckedInAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicAppointments_AppointmentSlots_AppointmentSlotId",
                        column: x => x.AppointmentSlotId,
                        principalTable: "AppointmentSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClinicAppointments_Ophthalmologists_AssignedDoctorId",
                        column: x => x.AssignedDoctorId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ClinicAppointments_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClinicAppointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_Capacity",
                table: "AppointmentSlots",
                columns: new[] { "Status", "BookedCount", "MaxCapacity" });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicAppointments_AppointmentSlotId",
                table: "ClinicAppointments",
                column: "AppointmentSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicAppointments_AssignedDoctorId",
                table: "ClinicAppointments",
                column: "AssignedDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicAppointments_OrganisationId",
                table: "ClinicAppointments",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicAppointments_Patient_Slot_Unique",
                table: "ClinicAppointments",
                columns: new[] { "PatientId", "AppointmentSlotId" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Status\" != 'Cancelled'");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicAppointments_PatientId",
                table: "ClinicAppointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicAppointments_Status",
                table: "ClinicAppointments",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicAppointments");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentSlots_Capacity",
                table: "AppointmentSlots");

            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "AppointmentSlots");
        }
    }
}
