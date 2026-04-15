using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOphthalmologistEmploymentTypeChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OphthalmologistEmploymentTypeChangeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OphthalmologistId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentEmploymentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TargetEmploymentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AdminNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReviewedByAdminUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OphthalmologistEmploymentTypeChangeRequests", x => x.Id);
                    table.CheckConstraint("CK_OphthalmologistEmploymentTypeChangeRequests_TargetDifferent", "\"CurrentEmploymentType\" <> \"TargetEmploymentType\"");
                    table.ForeignKey(
                        name: "FK_OphthalmologistEmploymentTypeChangeRequests_Ophthalmologist~",
                        column: x => x.OphthalmologistId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OphthalmologistEmploymentTypeChangeRequests_CreatedAt",
                table: "OphthalmologistEmploymentTypeChangeRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OphthalmologistEmploymentTypeChangeRequests_Ophthalmologist~",
                table: "OphthalmologistEmploymentTypeChangeRequests",
                column: "OphthalmologistId");

            migrationBuilder.CreateIndex(
                name: "IX_OphthalmologistEmploymentTypeChangeRequests_Status",
                table: "OphthalmologistEmploymentTypeChangeRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "UX_OphthalmologistEmploymentTypeChangeRequests_Pending",
                table: "OphthalmologistEmploymentTypeChangeRequests",
                columns: new[] { "OphthalmologistId", "Status" },
                unique: true,
                filter: "\"Status\" = 'Pending'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OphthalmologistEmploymentTypeChangeRequests");
        }
    }
}
