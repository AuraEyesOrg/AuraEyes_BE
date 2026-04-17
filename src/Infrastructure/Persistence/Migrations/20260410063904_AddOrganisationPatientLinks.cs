using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganisationPatientLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationPatientLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FirstLinkedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationPatientLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationPatientLinks_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganisationPatientLinks_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationPatientLinks_LastSeenAt",
                table: "OrganisationPatientLinks",
                column: "LastSeenAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationPatientLinks_OrganisationId",
                table: "OrganisationPatientLinks",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationPatientLinks_OrganisationId_PatientId",
                table: "OrganisationPatientLinks",
                columns: new[] { "OrganisationId", "PatientId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationPatientLinks_PatientId",
                table: "OrganisationPatientLinks",
                column: "PatientId");

            migrationBuilder.Sql(
                """
INSERT INTO "OrganisationPatientLinks" ("Id", "OrganisationId", "PatientId", "Source", "FirstLinkedAt", "LastSeenAt", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted")
WITH appointment_pairs AS (
        SELECT
                a."OrganisationId" AS "OrganisationId",
                a."PatientId" AS "PatientId",
                MIN(a."CreatedAt") AS "FirstLinkedAt",
                MAX(COALESCE(a."UpdatedAt", a."CreatedAt")) AS "LastSeenAt"
        FROM "Appointments" a
        WHERE a."OrganisationId" IS NOT NULL
            AND a."IsDeleted" = false
        GROUP BY a."OrganisationId", a."PatientId"
)
SELECT
    (
                substr(md5('appointment-' || ap."OrganisationId"::text || '-' || ap."PatientId"::text), 1, 8) || '-' ||
                substr(md5('appointment-' || ap."OrganisationId"::text || '-' || ap."PatientId"::text), 9, 4) || '-' ||
                substr(md5('appointment-' || ap."OrganisationId"::text || '-' || ap."PatientId"::text), 13, 4) || '-' ||
                substr(md5('appointment-' || ap."OrganisationId"::text || '-' || ap."PatientId"::text), 17, 4) || '-' ||
                substr(md5('appointment-' || ap."OrganisationId"::text || '-' || ap."PatientId"::text), 21, 12)
    )::uuid,
        ap."OrganisationId",
        ap."PatientId",
    'appointment-backfill',
        ap."FirstLinkedAt",
        ap."LastSeenAt",
        ap."FirstLinkedAt",
        ap."LastSeenAt",
    NULL,
    NULL,
    false
FROM appointment_pairs ap
WHERE NOT EXISTS (
            SELECT 1
            FROM "OrganisationPatientLinks" l
            WHERE l."OrganisationId" = ap."OrganisationId"
                AND l."PatientId" = ap."PatientId"
                AND l."IsDeleted" = false
    );

INSERT INTO "OrganisationPatientLinks" ("Id", "OrganisationId", "PatientId", "Source", "FirstLinkedAt", "LastSeenAt", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted")
WITH consultation_pairs AS (
        SELECT
                c."OrganisationId" AS "OrganisationId",
                c."PatientId" AS "PatientId",
                MIN(c."CreatedAt") AS "FirstLinkedAt",
                MAX(COALESCE(c."UpdatedAt", c."CreatedAt")) AS "LastSeenAt"
        FROM "ConsultationSessions" c
        WHERE c."OrganisationId" IS NOT NULL
            AND c."PatientId" IS NOT NULL
            AND c."IsDeleted" = false
        GROUP BY c."OrganisationId", c."PatientId"
)
SELECT
        (
                substr(md5('consultation-' || cp."OrganisationId"::text || '-' || cp."PatientId"::text), 1, 8) || '-' ||
                substr(md5('consultation-' || cp."OrganisationId"::text || '-' || cp."PatientId"::text), 9, 4) || '-' ||
                substr(md5('consultation-' || cp."OrganisationId"::text || '-' || cp."PatientId"::text), 13, 4) || '-' ||
                substr(md5('consultation-' || cp."OrganisationId"::text || '-' || cp."PatientId"::text), 17, 4) || '-' ||
                substr(md5('consultation-' || cp."OrganisationId"::text || '-' || cp."PatientId"::text), 21, 12)
        )::uuid,
        cp."OrganisationId",
        cp."PatientId",
        'consultation-backfill',
        cp."FirstLinkedAt",
        cp."LastSeenAt",
        cp."FirstLinkedAt",
        cp."LastSeenAt",
        NULL,
        NULL,
        false
FROM consultation_pairs cp
WHERE NOT EXISTS (
      SELECT 1
      FROM "OrganisationPatientLinks" l
            WHERE l."OrganisationId" = cp."OrganisationId"
                AND l."PatientId" = cp."PatientId"
        AND l."IsDeleted" = false
  );

INSERT INTO "OrganisationPatientLinks" ("Id", "OrganisationId", "PatientId", "Source", "FirstLinkedAt", "LastSeenAt", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted")
WITH legacy_pairs AS (
        SELECT
                u."OrganizationId" AS "OrganisationId",
                p."Id" AS "PatientId",
                MIN(u."CreatedAt") AS "FirstLinkedAt",
                MAX(COALESCE(u."UpdatedAt", u."CreatedAt")) AS "LastSeenAt"
        FROM "Patients" p
        JOIN "AspNetUsers" u ON p."UserId" = u."Id"
        WHERE u."OrganizationId" IS NOT NULL
            AND u."IsDeleted" = false
            AND p."IsDeleted" = false
        GROUP BY u."OrganizationId", p."Id"
)
SELECT
    (
                substr(md5('legacy-user-org-' || lp."OrganisationId"::text || '-' || lp."PatientId"::text), 1, 8) || '-' ||
                substr(md5('legacy-user-org-' || lp."OrganisationId"::text || '-' || lp."PatientId"::text), 9, 4) || '-' ||
                substr(md5('legacy-user-org-' || lp."OrganisationId"::text || '-' || lp."PatientId"::text), 13, 4) || '-' ||
                substr(md5('legacy-user-org-' || lp."OrganisationId"::text || '-' || lp."PatientId"::text), 17, 4) || '-' ||
                substr(md5('legacy-user-org-' || lp."OrganisationId"::text || '-' || lp."PatientId"::text), 21, 12)
    )::uuid,
        lp."OrganisationId",
        lp."PatientId",
    'legacy-user-org',
        lp."FirstLinkedAt",
        lp."LastSeenAt",
        lp."FirstLinkedAt",
        lp."LastSeenAt",
    NULL,
    NULL,
    false
FROM legacy_pairs lp
WHERE lp."OrganisationId" IS NOT NULL
  AND NOT EXISTS (
      SELECT 1
      FROM "OrganisationPatientLinks" l
            WHERE l."OrganisationId" = lp."OrganisationId"
                AND l."PatientId" = lp."PatientId"
        AND l."IsDeleted" = false
  );
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationPatientLinks");
        }
    }
}
