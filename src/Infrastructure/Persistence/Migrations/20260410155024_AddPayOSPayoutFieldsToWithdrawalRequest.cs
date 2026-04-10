using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPayOSPayoutFieldsToWithdrawalRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankBin",
                table: "WithdrawalRequests",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExternalPayoutId",
                table: "WithdrawalRequests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "WithdrawalRequests",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayOSApprovalState",
                table: "WithdrawalRequests",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayOSReferenceId",
                table: "WithdrawalRequests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayOSTransactionId",
                table: "WithdrawalRequests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankBin",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "ExternalPayoutId",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "Fee",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "PayOSApprovalState",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "PayOSReferenceId",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "PayOSTransactionId",
                table: "WithdrawalRequests");
        }
    }
}
