using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHNU_Connect.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixActorIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add ActorId column if it doesn't exist yet
            migrationBuilder.Sql(@"
                ALTER TABLE notifications
                ADD COLUMN IF NOT EXISTS ""ActorId"" integer;
            ");

            // Add index if it doesn't exist
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_notifications_ActorId""
                ON notifications (""ActorId"");
            ");

            // Add FK with SetNull if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.table_constraints
                        WHERE constraint_name = 'FK_notifications_users_ActorId'
                          AND table_name = 'notifications'
                    ) THEN
                        ALTER TABLE notifications
                        ADD CONSTRAINT ""FK_notifications_users_ActorId""
                        FOREIGN KEY (""ActorId"") REFERENCES users(id)
                        ON DELETE SET NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE notifications
                DROP CONSTRAINT IF EXISTS ""FK_notifications_users_ActorId"";
            ");

            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ""IX_notifications_ActorId"";
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE notifications
                DROP COLUMN IF EXISTS ""ActorId"";
            ");
        }
    }
}
