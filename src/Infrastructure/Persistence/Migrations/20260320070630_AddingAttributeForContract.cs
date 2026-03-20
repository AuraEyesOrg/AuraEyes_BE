using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingAttributeForContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmploymentType",
                table: "Ophthalmologists",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "FullTime");

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedMonthlySalary",
                table: "Ophthalmologists",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingHoursPerWeek",
                table: "Ophthalmologists",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmploymentType",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "ExpectedMonthlySalary",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "WorkingHoursPerWeek",
                table: "Ophthalmologists");
        }
    }
}
