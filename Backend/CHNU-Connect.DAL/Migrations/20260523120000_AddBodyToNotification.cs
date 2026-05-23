using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHNU_Connect.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBodyToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE notifications
                ADD COLUMN IF NOT EXISTS ""Body"" text;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE notifications
                DROP COLUMN IF EXISTS ""Body"";
            ");
        }
    }
}
