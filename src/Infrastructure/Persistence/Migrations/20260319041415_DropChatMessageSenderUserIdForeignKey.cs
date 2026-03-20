using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DropChatMessageSenderUserIdForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "ChatMessages"
                DROP CONSTRAINT IF EXISTS "ChatMessages_SenderUserId_fkey";
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty — do not re-create a broken FK.
        }
    }
}
