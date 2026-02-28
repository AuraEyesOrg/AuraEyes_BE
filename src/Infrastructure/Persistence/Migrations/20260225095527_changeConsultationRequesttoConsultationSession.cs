using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeConsultationRequesttoConsultationSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ConsultationRequests_ConsultationRequestId",
                table: "Conversations");

            migrationBuilder.DropTable(
                name: "ConsultationRequests");

            migrationBuilder.RenameColumn(
                name: "ConsultationRequestId",
                table: "Conversations",
                newName: "ConsultationSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_ConsultationRequestId",
                table: "Conversations",
                newName: "IX_Conversations_ConsultationSessionId");

            migrationBuilder.AddColumn<Guid>(
                name: "ConsultationSessionId",
                table: "MedicalDiagnoses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConsultationSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    OphthalmologistId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: true),
                    AiScreeningId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ChatStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AppointmentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MeetingLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LastActivityAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ClosingReason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationSessions_AiScreenings_AiScreeningId",
                        column: x => x.AiScreeningId,
                        principalTable: "AiScreenings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationSessions_Ophthalmologists_OphthalmologistId",
                        column: x => x.OphthalmologistId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationSessions_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationSessions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDiagnoses_ConsultationSessionId",
                table: "MedicalDiagnoses",
                column: "ConsultationSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_AiScreeningId",
                table: "ConsultationSessions",
                column: "AiScreeningId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_OphthalmologistId",
                table: "ConsultationSessions",
                column: "OphthalmologistId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_OrganisationId",
                table: "ConsultationSessions",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_PatientId",
                table: "ConsultationSessions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_StaleSessionLookup",
                table: "ConsultationSessions",
                columns: new[] { "Status", "ChatStatus", "LastActivityAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_ConsultationSessions_ConsultationSessionId",
                table: "Conversations",
                column: "ConsultationSessionId",
                principalTable: "ConsultationSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalDiagnoses_ConsultationSessions_ConsultationSessionId",
                table: "MedicalDiagnoses",
                column: "ConsultationSessionId",
                principalTable: "ConsultationSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ConsultationSessions_ConsultationSessionId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalDiagnoses_ConsultationSessions_ConsultationSessionId",
                table: "MedicalDiagnoses");

            migrationBuilder.DropTable(
                name: "ConsultationSessions");

            migrationBuilder.DropIndex(
                name: "IX_MedicalDiagnoses_ConsultationSessionId",
                table: "MedicalDiagnoses");

            migrationBuilder.DropColumn(
                name: "ConsultationSessionId",
                table: "MedicalDiagnoses");

            migrationBuilder.RenameColumn(
                name: "ConsultationSessionId",
                table: "Conversations",
                newName: "ConsultationRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_ConsultationSessionId",
                table: "Conversations",
                newName: "IX_Conversations_ConsultationRequestId");

            migrationBuilder.CreateTable(
                name: "ConsultationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    DiagnosisNote = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsFeedbackRequested = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    MeetingLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequestMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationRequests_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_PatientId",
                table: "ConsultationRequests",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_ConsultationRequests_ConsultationRequestId",
                table: "Conversations",
                column: "ConsultationRequestId",
                principalTable: "ConsultationRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
