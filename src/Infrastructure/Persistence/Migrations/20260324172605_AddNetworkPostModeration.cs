using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNetworkPostModeration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HideReason",
                table: "ProfessionalPosts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "ProfessionalPosts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalPosts_IsHidden",
                table: "ProfessionalPosts",
                column: "IsHidden");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfessionalPosts_IsHidden",
                table: "ProfessionalPosts");

            migrationBuilder.DropColumn(
                name: "HideReason",
                table: "ProfessionalPosts");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "ProfessionalPosts");
        }
    }
}
