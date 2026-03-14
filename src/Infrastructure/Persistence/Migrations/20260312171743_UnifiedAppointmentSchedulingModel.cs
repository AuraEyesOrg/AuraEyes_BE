using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UnifiedAppointmentSchedulingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "AvailableSlots");

            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentSlotId",
                table: "ConsultationSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScheduleTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrgId = table.Column<Guid>(type: "uuid", nullable: true),
                    OphthalId = table.Column<Guid>(type: "uuid", nullable: true),
                    DayOfWeek = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    SlotDuration = table.Column<int>(type: "integer", nullable: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    BookedCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ReservedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ReservationExpireAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentSlots_ScheduleTemplates_ScheduleTemplateId",
                        column: x => x.ScheduleTemplateId,
                        principalTable: "ScheduleTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: true),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConsultationSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VisitReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsRetinalImagesShared = table.Column<bool>(type: "boolean", nullable: false),
                    IsAiResultShared = table.Column<bool>(type: "boolean", nullable: false),
                    CheckedInAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_AppointmentSlots_AppointmentSlotId",
                        column: x => x.AppointmentSlotId,
                        principalTable: "AppointmentSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_ConsultationSessions_ConsultationSessionId",
                        column: x => x.ConsultationSessionId,
                        principalTable: "ConsultationSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Appointments_Ophthalmologists_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Appointments_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_AppointmentSlotId",
                table: "ConsultationSessions",
                column: "AppointmentSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentSlotId",
                table: "Appointments",
                column: "AppointmentSlotId");

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
                name: "IX_Appointments_Patient_Slot_Unique",
                table: "Appointments",
                columns: new[] { "PatientId", "AppointmentSlotId" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Status\" NOT IN ('Cancelled', 'NoShow')");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Status",
                table: "Appointments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Type",
                table: "Appointments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Type_Status",
                table: "Appointments",
                columns: new[] { "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_Capacity",
                table: "AppointmentSlots",
                columns: new[] { "Status", "BookedCount", "MaxCapacity" });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_Date",
                table: "AppointmentSlots",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_ScheduleTemplateId",
                table: "AppointmentSlots",
                column: "ScheduleTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_Status",
                table: "AppointmentSlots",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_Status_ReservationExpireAt",
                table: "AppointmentSlots",
                columns: new[] { "Status", "ReservationExpireAt" },
                filter: "\"Status\" = 'Reserved'");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTemplates_DayOfWeek",
                table: "ScheduleTemplates",
                column: "DayOfWeek");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTemplates_OphthalId",
                table: "ScheduleTemplates",
                column: "OphthalId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTemplates_OrgId",
                table: "ScheduleTemplates",
                column: "OrgId");

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

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "AppointmentSlots");

            migrationBuilder.DropTable(
                name: "ScheduleTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationSessions_AppointmentSlotId",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "AppointmentSlotId",
                table: "ConsultationSessions");

            migrationBuilder.CreateTable(
                name: "AvailableSlots",
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

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AvailableSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_AvailableSlots_AvailableSlotId",
                        column: x => x.AvailableSlotId,
                        principalTable: "AvailableSlots",
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

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_AvailableSlotId",
                table: "Schedules",
                column: "AvailableSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_PatientId",
                table: "Schedules",
                column: "PatientId");
        }
    }
}
