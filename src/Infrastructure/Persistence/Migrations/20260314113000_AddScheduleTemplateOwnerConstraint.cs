using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260314113000_AddScheduleTemplateOwnerConstraint")]
    public partial class AddScheduleTemplateOwnerConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"ScheduleTemplates\" " +
                "ADD CONSTRAINT \"CK_ScheduleTemplates_OphthalWithoutOrg\" " +
                "CHECK (\"OphthalId\" IS NULL OR \"OrgId\" IS NULL);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"ScheduleTemplates\" " +
                "DROP CONSTRAINT IF EXISTS \"CK_ScheduleTemplates_OphthalWithoutOrg\";");
        }
    }
}
