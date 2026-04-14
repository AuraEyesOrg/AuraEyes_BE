using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOphthalmologistLeaveRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OphthalmologistLeaveRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OphthalmologistId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
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
                    table.PrimaryKey("PK_OphthalmologistLeaveRequests", x => x.Id);
                    table.CheckConstraint("CK_OphthalmologistLeaveRequests_DateRange", "\"EndDate\" >= \"StartDate\"");
                    table.ForeignKey(
                        name: "FK_OphthalmologistLeaveRequests_Ophthalmologists_Ophthalmologi~",
                        column: x => x.OphthalmologistId,
                        principalTable: "Ophthalmologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OphthalmologistLeaveRequests_CreatedAt",
                table: "OphthalmologistLeaveRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OphthalmologistLeaveRequests_OphthalmologistId",
                table: "OphthalmologistLeaveRequests",
                column: "OphthalmologistId");

            migrationBuilder.CreateIndex(
                name: "IX_OphthalmologistLeaveRequests_OphthalmologistId_StartDate_En~",
                table: "OphthalmologistLeaveRequests",
                columns: new[] { "OphthalmologistId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_OphthalmologistLeaveRequests_Status",
                table: "OphthalmologistLeaveRequests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OphthalmologistLeaveRequests");
        }
    }
}
