using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationSessions_Organisations_OrganisationId",
                table: "ConsultationSessions");

            migrationBuilder.DropTable(
                name: "OrganisationFeedback");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationSessions_OrganisationId",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "ConsultationSessions");

            migrationBuilder.AlterColumn<int>(
                name: "UsedAiQuota",
                table: "Patients",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "PurchasedAiQuota",
                table: "Patients",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Patients",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ClinicalDataJson",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AdministrativeDataJson",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ClinicFeedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: true),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicFeedback", x => x.Id);
                    table.CheckConstraint("CK_ClinicFeedback_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5");
                    table.ForeignKey(
                        name: "FK_ClinicFeedback_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClinicFeedback_ClinicStaffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "ClinicStaffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClinicFeedback_Ophthalmologists_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClinicFeedback_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicFeedback_AppointmentId",
                table: "ClinicFeedback",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicFeedback_DoctorId",
                table: "ClinicFeedback",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicFeedback_PatientId",
                table: "ClinicFeedback",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicFeedback_StaffId",
                table: "ClinicFeedback",
                column: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicFeedback");

            migrationBuilder.AlterColumn<int>(
                name: "UsedAiQuota",
                table: "Patients",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "PurchasedAiQuota",
                table: "Patients",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Patients",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "ClinicalDataJson",
                table: "MedicalRecords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AdministrativeDataJson",
                table: "MedicalRecords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "ConsultationSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrganisationFeedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    organisation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationFeedback", x => x.Id);
                    table.CheckConstraint("CK_OrganisationFeedback_Rating", "rating >= 1 AND rating <= 5");
                    table.ForeignKey(
                        name: "FK_OrganisationFeedback_Appointments_appointment_id",
                        column: x => x.appointment_id,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganisationFeedback_Organisations_organisation_id",
                        column: x => x.organisation_id,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganisationFeedback_Patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_OrganisationId",
                table: "ConsultationSessions",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFeedback_appointment_id",
                table: "OrganisationFeedback",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFeedback_created_at",
                table: "OrganisationFeedback",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFeedback_organisation_id",
                table: "OrganisationFeedback",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFeedback_patient_id_appointment_id",
                table: "OrganisationFeedback",
                columns: new[] { "patient_id", "appointment_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationSessions_Organisations_OrganisationId",
                table: "ConsultationSessions",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
