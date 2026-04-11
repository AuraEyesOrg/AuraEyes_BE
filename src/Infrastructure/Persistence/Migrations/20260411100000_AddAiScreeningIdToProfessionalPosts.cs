using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260411100000_AddAiScreeningIdToProfessionalPosts")]
    public partial class AddAiScreeningIdToProfessionalPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AiScreeningId",
                table: "ProfessionalPosts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalPosts_AiScreeningId",
                table: "ProfessionalPosts",
                column: "AiScreeningId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfessionalPosts_AiScreeningId",
                table: "ProfessionalPosts");

            migrationBuilder.DropColumn(
                name: "AiScreeningId",
                table: "ProfessionalPosts");
        }
    }
}
