using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceClinicVisitDoctorNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Appointments_ClinicVisit_DoctorId_Null",
                table: "Appointments",
                sql: "\"Type\" <> 'ClinicVisit' OR \"DoctorId\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Appointments_ClinicVisit_DoctorId_Null",
                table: "Appointments");
        }
    }
}
