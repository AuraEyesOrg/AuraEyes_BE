using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOphthalmologistDealTermsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"Ophthalmologists\" ADD COLUMN IF NOT EXISTS \"CommissionRate\" numeric(5,2) NULL;");

            migrationBuilder.Sql(
                "ALTER TABLE \"Ophthalmologists\" ADD COLUMN IF NOT EXISTS \"ActualMonthlySalary\" numeric(18,2) NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"Ophthalmologists\" DROP COLUMN IF EXISTS \"CommissionRate\";");

            migrationBuilder.Sql(
                "ALTER TABLE \"Ophthalmologists\" DROP COLUMN IF EXISTS \"ActualMonthlySalary\";");
        }
    }
}
