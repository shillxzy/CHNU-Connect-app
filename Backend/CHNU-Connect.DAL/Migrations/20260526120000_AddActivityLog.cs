using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHNU_Connect.DAL.Migrations
{
    public partial class AddActivityLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS activity_logs (
                    id SERIAL PRIMARY KEY,
                    user_id INTEGER NOT NULL,
                    user_name VARCHAR(255) NOT NULL DEFAULT '',
                    action VARCHAR(100) NOT NULL,
                    entity_type VARCHAR(50),
                    entity_id INTEGER,
                    timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW()
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS activity_logs;");
        }
    }
}
