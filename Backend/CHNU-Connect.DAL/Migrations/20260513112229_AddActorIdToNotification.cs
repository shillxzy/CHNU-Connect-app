using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CHNU_Connect.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddActorIdToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lesson_slots",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pair_number = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lesson_slots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "admin_actions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    admin_id = table.Column<int>(type: "integer", nullable: false),
                    target_user_id = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_actions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chat_members",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chat_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "member"),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    last_read_message_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_members", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chat_messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chat_id = table.Column<int>(type: "integer", nullable: false),
                    sender_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    message_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "text"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    direct_key = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "event_participants",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_participants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<int>(type: "integer", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "group_members",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_members", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<int>(type: "integer", nullable: false),
                    is_private = table.Column<bool>(type: "boolean", nullable: false),
                    curator_id = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sub_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sub_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_sub_groups_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    faculty = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    course = table.Column<int>(type: "integer", nullable: true),
                    photo_url = table.Column<string>(type: "text", nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    IsEmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    sub_group_id = table.Column<int>(type: "integer", nullable: true),
                    EmailConfirmationToken = table.Column<string>(type: "text", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "text", nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_sub_groups_sub_group_id",
                        column: x => x.sub_group_id,
                        principalTable: "sub_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sender_id = table.Column<int>(type: "integer", nullable: false),
                    receiver_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_messages_users_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    ActorId = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<string>(type: "text", nullable: false),
                    entity_id = table.Column<int>(type: "integer", nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_users_ActorId",
                        column: x => x.ActorId,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.id);
                    table.ForeignKey(
                        name: "FK_posts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    teacher_id = table.Column<int>(type: "integer", nullable: true),
                    moodle_link = table.Column<string>(type: "text", nullable: true),
                    semester = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subjects", x => x.id);
                    table.ForeignKey(
                        name: "FK_subjects_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_subjects_users_teacher_id",
                        column: x => x.teacher_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "post_likes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_likes_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_post_likes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    subject_id = table.Column<int>(type: "integer", nullable: true),
                    sub_group_id = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    week = table.Column<int>(type: "integer", nullable: false),
                    is_every_week = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    day = table.Column<int>(type: "integer", nullable: false),
                    slot_id = table.Column<int>(type: "integer", nullable: false),
                    subject_name = table.Column<string>(type: "text", nullable: false),
                    teacher_name = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedules", x => x.id);
                    table.ForeignKey(
                        name: "FK_schedules_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_schedules_lesson_slots_slot_id",
                        column: x => x.slot_id,
                        principalTable: "lesson_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_schedules_sub_groups_sub_group_id",
                        column: x => x.sub_group_id,
                        principalTable: "sub_groups",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_schedules_subjects_subject_id",
                        column: x => x.subject_id,
                        principalTable: "subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_actions_admin_id",
                table: "admin_actions",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_actions_target_user_id",
                table: "admin_actions",
                column: "target_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_members_chat_id_user_id",
                table: "chat_members",
                columns: new[] { "chat_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chat_members_user_id",
                table: "chat_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_chat_id",
                table: "chat_messages",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_sender_id",
                table: "chat_messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_chats_created_by",
                table: "chats",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_comments_post_id",
                table: "comments",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_user_id",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_participants_event_id_user_id",
                table: "event_participants",
                columns: new[] { "event_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_participants_user_id",
                table: "event_participants",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_creator_id",
                table: "events",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_members_group_id_user_id",
                table: "group_members",
                columns: new[] { "group_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_members_user_id",
                table: "group_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_creator_id",
                table: "groups",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_curator_id",
                table: "groups",
                column: "curator_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_name",
                table: "groups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_messages_receiver_id",
                table: "messages",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_sender_id",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_ActorId",
                table: "notifications",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_post_id_user_id",
                table: "post_likes",
                columns: new[] { "post_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_user_id",
                table: "post_likes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_user_id",
                table: "posts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedules_group_id",
                table: "schedules",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedules_slot_id",
                table: "schedules",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedules_sub_group_id",
                table: "schedules",
                column: "sub_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedules_subject_id",
                table: "schedules",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_sub_groups_group_id",
                table: "sub_groups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_subjects_group_id",
                table: "subjects",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_subjects_teacher_id",
                table: "subjects",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_sub_group_id",
                table: "users",
                column: "sub_group_id");

            migrationBuilder.AddForeignKey(
                name: "FK_admin_actions_users_admin_id",
                table: "admin_actions",
                column: "admin_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_admin_actions_users_target_user_id",
                table: "admin_actions",
                column: "target_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_chat_members_chats_chat_id",
                table: "chat_members",
                column: "chat_id",
                principalTable: "chats",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chat_members_users_user_id",
                table: "chat_members",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chat_messages_chats_chat_id",
                table: "chat_messages",
                column: "chat_id",
                principalTable: "chats",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chat_messages_users_sender_id",
                table: "chat_messages",
                column: "sender_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chats_users_created_by",
                table: "chats",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_posts_post_id",
                table: "comments",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_users_user_id",
                table: "comments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_participants_events_event_id",
                table: "event_participants",
                column: "event_id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_participants_users_user_id",
                table: "event_participants",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_events_users_creator_id",
                table: "events",
                column: "creator_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_group_members_groups_group_id",
                table: "group_members",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_group_members_users_user_id",
                table: "group_members",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_groups_users_creator_id",
                table: "groups",
                column: "creator_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_groups_users_curator_id",
                table: "groups",
                column: "curator_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_groups_users_creator_id",
                table: "groups");

            migrationBuilder.DropForeignKey(
                name: "FK_groups_users_curator_id",
                table: "groups");

            migrationBuilder.DropTable(
                name: "admin_actions");

            migrationBuilder.DropTable(
                name: "chat_members");

            migrationBuilder.DropTable(
                name: "chat_messages");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "event_participants");

            migrationBuilder.DropTable(
                name: "group_members");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "post_likes");

            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropTable(
                name: "chats");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "lesson_slots");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "sub_groups");

            migrationBuilder.DropTable(
                name: "groups");
        }
    }
}
