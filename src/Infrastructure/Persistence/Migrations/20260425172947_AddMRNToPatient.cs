using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMRNToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Defensive check: Add column only if it doesn't exist to resolve schema out-of-sync issues
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                                   WHERE table_name='Patients' AND column_name='MedicalRecordNumber') THEN 
                        ALTER TABLE ""Patients"" ADD COLUMN ""MedicalRecordNumber"" text; 
                    END IF; 
                END $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicalRecordNumber",
                table: "Patients");
        }
    }
}
