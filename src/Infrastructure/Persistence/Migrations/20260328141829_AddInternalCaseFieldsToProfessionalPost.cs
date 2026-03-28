using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInternalCaseFieldsToProfessionalPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConsultationSessionId",
                table: "ProfessionalPosts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInternalCase",
                table: "ProfessionalPosts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PatientAge",
                table: "ProfessionalPosts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientGender",
                table: "ProfessionalPosts",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalPosts_ConsultationSessionId",
                table: "ProfessionalPosts",
                column: "ConsultationSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalPosts_IsInternalCase",
                table: "ProfessionalPosts",
                column: "IsInternalCase");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfessionalPosts_ConsultationSessionId",
                table: "ProfessionalPosts");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalPosts_IsInternalCase",
                table: "ProfessionalPosts");

            migrationBuilder.DropColumn(
                name: "ConsultationSessionId",
                table: "ProfessionalPosts");

            migrationBuilder.DropColumn(
                name: "IsInternalCase",
                table: "ProfessionalPosts");

            migrationBuilder.DropColumn(
                name: "PatientAge",
                table: "ProfessionalPosts");

            migrationBuilder.DropColumn(
                name: "PatientGender",
                table: "ProfessionalPosts");
        }
    }
}
