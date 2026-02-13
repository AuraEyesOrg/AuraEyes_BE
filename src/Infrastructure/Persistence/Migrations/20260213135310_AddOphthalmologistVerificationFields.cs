using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOphthalmologistVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Follows");

            migrationBuilder.AddColumn<string>(
                name: "DegreeUrl",
                table: "Ophthalmologists",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseUrl",
                table: "Ophthalmologists",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Ophthalmologists",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Ophthalmologists",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "Ophthalmologists",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "PendingVerification");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegreeUrl",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "LicenseUrl",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Ophthalmologists");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "Ophthalmologists");

            migrationBuilder.CreateTable(
                name: "Follows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    FollowerId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowerType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FollowingId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowingType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    NotificationsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Follows", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowerId",
                table: "Follows",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowerId_FollowerType",
                table: "Follows",
                columns: new[] { "FollowerId", "FollowerType" });

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowerId_FollowingId",
                table: "Follows",
                columns: new[] { "FollowerId", "FollowingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowingId",
                table: "Follows",
                column: "FollowingId");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowingId_FollowingType",
                table: "Follows",
                columns: new[] { "FollowingId", "FollowingType" });
        }
    }
}
