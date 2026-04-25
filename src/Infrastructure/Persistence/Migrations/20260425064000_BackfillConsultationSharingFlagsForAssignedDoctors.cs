using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Backfills sharing flags so legacy assigned verification/clinic sessions expose screening data.
    /// </summary>
    public partial class BackfillConsultationSharingFlagsForAssignedDoctors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE "ConsultationSessions"
                SET
                    "IsRetinalImagesShared" = TRUE,
                    "IsAIResultShared" = TRUE,
                    "UpdatedAt" = NOW()
                WHERE
                    "OphthalmologistId" IS NOT NULL
                    AND "Type" IN (1, 3)
                    AND ("IsRetinalImagesShared" = FALSE OR "IsAIResultShared" = FALSE);
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill migration: no rollback to avoid removing already-enabled access.
        }
    }
}
