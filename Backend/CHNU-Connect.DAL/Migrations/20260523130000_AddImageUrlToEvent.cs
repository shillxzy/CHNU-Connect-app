using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHNU_Connect.DAL.Migrations
{
    public partial class AddImageUrlToEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE events
                ADD COLUMN IF NOT EXISTS ""ImageUrl"" text;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE events
                DROP COLUMN IF EXISTS ""ImageUrl"";
            ");
        }
    }
}
