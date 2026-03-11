using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SchemaRedesign_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consents_Patients_PatientId",
                table: "Consents");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Ophthalmologists_OphthalmologistId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Organisations_OrganisationId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_OrganisationId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Consents_PatientId",
                table: "Consents");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "MedicalHistorySummary",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "OphthalmologistId",
                table: "Schedules",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_OphthalmologistId",
                table: "Schedules",
                newName: "IX_Schedules_PatientId");

            migrationBuilder.RenameColumn(
                name: "ReferralRequired",
                table: "MedicalDiagnoses",
                newName: "IsReferralNeeded");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ConsultationSessions",
                newName: "PlatformFee");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Consents",
                newName: "AiScreeningId");

            migrationBuilder.RenameColumn(
                name: "IsValid",
                table: "Consents",
                newName: "IsAgreed");

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceId",
                table: "WalletTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "WalletTransactions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerType",
                table: "Wallets",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsGranted",
                table: "UserPermissions",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AvailabilityId",
                table: "Schedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "BMI",
                table: "Patients",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiseaseHistory",
                table: "Patients",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ConsultationSessionId",
                table: "MedicalDiagnoses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LifestyleAdvice",
                table: "MedicalDiagnoses",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "Feedbacks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetType",
                table: "Feedbacks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AiQuotaLimit",
                table: "Contracts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PlatformCommissionRate",
                table: "Contracts",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

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

            migrationBuilder.AddColumn<decimal>(
                name: "OfflineClinicFee",
                table: "ConsultationSessions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Consents",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "AiScreenings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Availabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: true),
                    OphthalmologistId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Availabilities_Ophthalmologists_OphthalmologistId",
                        column: x => x.OphthalmologistId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Availabilities_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Key);
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Key", "Description", "Value" },
                values: new object[,]
                {
                    { "AI_QUOTA_BUNDLE", "Credits per paid bundle", "5" },
                    { "AI_QUOTA_PRICE", "Price per 5 additional AI credits (VND)", "50000" },
                    { "DEFAULT_PLATFORM_COMMISSION", "Default platform commission rate (20%)", "0.20" },
                    { "FREE_AI_QUOTA", "Free AI screening credits per patient", "3" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ReferenceType_ReferenceId",
                table: "WalletTransactions",
                columns: new[] { "ReferenceType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_AvailabilityId",
                table: "Schedules",
                column: "AvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_TargetType_TargetId",
                table: "Feedbacks",
                columns: new[] { "TargetType", "TargetId" });

            migrationBuilder.CreateIndex(
                name: "IX_Consents_AiScreeningId",
                table: "Consents",
                column: "AiScreeningId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AiScreenings_PatientId",
                table: "AiScreenings",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_OphthalmologistId_StartTime_EndTime",
                table: "Availabilities",
                columns: new[] { "OphthalmologistId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_OrganisationId_StartTime_EndTime",
                table: "Availabilities",
                columns: new[] { "OrganisationId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            // Clear existing Schedules — they reference old OphthalmologistId/OrganisationId
            // which are no longer valid after this schema redesign.
            migrationBuilder.Sql("DELETE FROM \"Schedules\";");
            // Also clear AiScreenings (and Consents via cascade) that have invalid default PatientId.
            migrationBuilder.Sql("DELETE FROM \"Consents\" WHERE \"AiScreeningId\" IN (SELECT \"Id\" FROM \"AiScreenings\" WHERE \"PatientId\" = '00000000-0000-0000-0000-000000000000');");
            migrationBuilder.Sql("DELETE FROM \"AiScreenings\" WHERE \"PatientId\" = '00000000-0000-0000-0000-000000000000';");

            migrationBuilder.AddForeignKey(
                name: "FK_AiScreenings_Patients_PatientId",
                table: "AiScreenings",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Consents_AiScreenings_AiScreeningId",
                table: "Consents",
                column: "AiScreeningId",
                principalTable: "AiScreenings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Availabilities_AvailabilityId",
                table: "Schedules",
                column: "AvailabilityId",
                principalTable: "Availabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiScreenings_Patients_PatientId",
                table: "AiScreenings");

            migrationBuilder.DropForeignKey(
                name: "FK_Consents_AiScreenings_AiScreeningId",
                table: "Consents");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Availabilities_AvailabilityId",
                table: "Schedules");

            migrationBuilder.DropTable(
                name: "Availabilities");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_ReferenceType_ReferenceId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_AvailabilityId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_TargetType_TargetId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Consents_AiScreeningId",
                table: "Consents");

            migrationBuilder.DropIndex(
                name: "IX_AiScreenings_PatientId",
                table: "AiScreenings");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "OwnerType",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "IsGranted",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "AvailabilityId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "BMI",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DiseaseHistory",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "LifestyleAdvice",
                table: "MedicalDiagnoses");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "AiQuotaLimit",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PlatformCommissionRate",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "IsAIResultShared",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "IsRetinalImagesShared",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "OfflineClinicFee",
                table: "ConsultationSessions");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "AiScreenings");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Schedules",
                newName: "OphthalmologistId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_PatientId",
                table: "Schedules",
                newName: "IX_Schedules_OphthalmologistId");

            migrationBuilder.RenameColumn(
                name: "IsReferralNeeded",
                table: "MedicalDiagnoses",
                newName: "ReferralRequired");

            migrationBuilder.RenameColumn(
                name: "PlatformFee",
                table: "ConsultationSessions",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "IsAgreed",
                table: "Consents",
                newName: "IsValid");

            migrationBuilder.RenameColumn(
                name: "AiScreeningId",
                table: "Consents",
                newName: "PatientId");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "Schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicalHistorySummary",
                table: "Patients",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ConsultationSessionId",
                table: "MedicalDiagnoses",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Consents",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_OrganisationId",
                table: "Schedules",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Consents_PatientId",
                table: "Consents",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consents_Patients_PatientId",
                table: "Consents",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Ophthalmologists_OphthalmologistId",
                table: "Schedules",
                column: "OphthalmologistId",
                principalTable: "Ophthalmologists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Organisations_OrganisationId",
                table: "Schedules",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
