using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganisationMonthlyQuotaAndOrgScreeningBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MonthlyQuotaLastResetAt",
                table: "Organisations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyQuotaLimit",
                table: "Organisations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyQuotaUsed",
                table: "Organisations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyQuotaLimit",
                table: "Contracts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "AiScreenings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AiScreenings_OrganisationId",
                table: "AiScreenings",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AiScreenings_Organisations_OrganisationId",
                table: "AiScreenings",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiScreenings_Organisations_OrganisationId",
                table: "AiScreenings");

            migrationBuilder.DropIndex(
                name: "IX_AiScreenings_OrganisationId",
                table: "AiScreenings");

            migrationBuilder.DropColumn(
                name: "MonthlyQuotaLastResetAt",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "MonthlyQuotaLimit",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "MonthlyQuotaUsed",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "MonthlyQuotaLimit",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "AiScreenings");
        }
    }
}
