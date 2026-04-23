using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ClinicCentricScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_ConsultationSessions_ConsultationSessionId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Ophthalmologists_DoctorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Organisations_OrganisationId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleTemplates_OphthalId",
                table: "ScheduleTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleTemplates_OrgId",
                table: "ScheduleTemplates");

            migrationBuilder.DropIndex(
                name: "UX_ScheduleTemplates_FullTime_SystemGenerated_Day",
                table: "ScheduleTemplates");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ScheduleTemplates_OphthalWithoutOrg",
                table: "ScheduleTemplates");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentSlots_Status_ReservationExpireAt",
                table: "AppointmentSlots");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ConsultationSessionId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_OrganisationId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_Type",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_Type_Status",
                table: "Appointments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Appointments_ClinicVisit_DoctorId_Null",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "OphthalId",
                table: "ScheduleTemplates");

            migrationBuilder.DropColumn(
                name: "OrgId",
                table: "ScheduleTemplates");

            migrationBuilder.DropColumn(
                name: "ReservationExpireAt",
                table: "AppointmentSlots");

            migrationBuilder.DropColumn(
                name: "ReservedBy",
                table: "AppointmentSlots");

            migrationBuilder.DropColumn(
                name: "CheckedInAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ConsultationSessionId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsAiResultShared",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsRetinalImagesShared",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Appointments");

            migrationBuilder.CreateTable(
                name: "PatientVisits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedDoctorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CheckedInAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientVisits_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientVisits_Ophthalmologists_AssignedDoctorId",
                        column: x => x.AssignedDoctorId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PatientVisits_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SlotAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlotAssignments_AppointmentSlots_AppointmentSlotId",
                        column: x => x.AppointmentSlotId,
                        principalTable: "AppointmentSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTemplates_IsActive",
                table: "ScheduleTemplates",
                column: "IsActive");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AppointmentSlots_BookedCount_Capacity",
                table: "AppointmentSlots",
                sql: "\"BookedCount\" <= \"MaxCapacity\"");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_AppointmentId",
                table: "PatientVisits",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_AssignedDoctorId",
                table: "PatientVisits",
                column: "AssignedDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_CheckedInAt",
                table: "PatientVisits",
                column: "CheckedInAt");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_PatientId",
                table: "PatientVisits",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_Status",
                table: "PatientVisits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SlotAssignments_AppointmentSlotId",
                table: "SlotAssignments",
                column: "AppointmentSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotAssignments_StaffId",
                table: "SlotAssignments",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "UX_SlotAssignments_Slot_Staff_Role",
                table: "SlotAssignments",
                columns: new[] { "AppointmentSlotId", "StaffId", "Role" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientVisits");

            migrationBuilder.DropTable(
                name: "SlotAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleTemplates_IsActive",
                table: "ScheduleTemplates");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AppointmentSlots_BookedCount_Capacity",
                table: "AppointmentSlots");

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

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedInAt",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConsultationSessionId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAiResultShared",
                table: "Appointments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRetinalImagesShared",
                table: "Appointments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Appointments",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Appointments",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTemplates_OphthalId",
                table: "ScheduleTemplates",
                column: "OphthalId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTemplates_OrgId",
                table: "ScheduleTemplates",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "UX_ScheduleTemplates_FullTime_SystemGenerated_Day",
                table: "ScheduleTemplates",
                columns: new[] { "OphthalId", "DayOfWeek" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"IsActive\" = true AND \"Source\" = 'SystemGenerated' AND \"OphthalId\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ScheduleTemplates_OphthalWithoutOrg",
                table: "ScheduleTemplates",
                sql: "\"OphthalId\" IS NULL OR \"OrgId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_Status_ReservationExpireAt",
                table: "AppointmentSlots",
                columns: new[] { "Status", "ReservationExpireAt" },
                filter: "\"Status\" = 'Reserved'");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ConsultationSessionId",
                table: "Appointments",
                column: "ConsultationSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_OrganisationId",
                table: "Appointments",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Type",
                table: "Appointments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Type_Status",
                table: "Appointments",
                columns: new[] { "Type", "Status" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Appointments_ClinicVisit_DoctorId_Null",
                table: "Appointments",
                sql: "\"Type\" <> 'ClinicVisit' OR \"DoctorId\" IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_ConsultationSessions_ConsultationSessionId",
                table: "Appointments",
                column: "ConsultationSessionId",
                principalTable: "ConsultationSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Ophthalmologists_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "Ophthalmologists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Organisations_OrganisationId",
                table: "Appointments",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
