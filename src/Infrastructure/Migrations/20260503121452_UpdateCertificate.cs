using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCertificate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IssuingInstitution",
                table: "Certificates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Certificates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScopeOfPractice",
                table: "Certificates",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuingInstitution",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ScopeOfPractice",
                table: "Certificates");
        }
    }
}
