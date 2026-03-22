using CHNU_Connect.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<PostLike> PostLikes { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;

        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<GroupMember> GroupMembers { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;
        public DbSet<SubGroup> SubGroups { get; set; } = null!;

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventParticipant> EventParticipants { get; set; } = null!;

        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<AdminAction> AdminActions { get; set; } = null!;

        public DbSet<Chat> Chats { get; set; } = null!;
        public DbSet<ChatMember> ChatMembers { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================= USERS =================
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()");

                // SubGroup
                entity.HasOne(e => e.SubGroup)
                    .WithMany(sg => sg.Users)
                    .HasForeignKey(e => e.SubGroupId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ================= GROUP =================
            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("groups");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.Type).IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()");

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Curator)
                    .WithMany()
                    .HasForeignKey(e => e.CuratorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ================= GROUP MEMBER =================
            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.ToTable("group_members");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Role)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.JoinedAt)
                    .HasDefaultValueSql("now()");

                entity.HasOne(e => e.Group)
                    .WithMany(g => g.Members)
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Groups)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.GroupId, e.UserId }).IsUnique();
            });

            // ================= SUBJECT =================
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("subjects");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(e => e.Group)
                    .WithMany(g => g.Subjects)
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Teacher)
                    .WithMany()
                    .HasForeignKey(e => e.TeacherId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ================= SUBGROUP =================
            modelBuilder.Entity<SubGroup>(entity =>
            {
                entity.ToTable("sub_groups");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(e => e.Group)
                    .WithMany()
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ================= SCHEDULE =================
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedules");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Day).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();

                entity.HasOne(e => e.Group)
                    .WithMany(g => g.Schedules)
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Subject)
                    .WithMany(s => s.Schedules)
                    .HasForeignKey(e => e.SubjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SubGroup)
                    .WithMany(sg => sg.Schedules)
                    .HasForeignKey(e => e.SubGroupId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ================= POSTS =================
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("posts");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ================= COMMENTS =================
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(e => e.PostId);
            });

            // ================= MESSAGES =================
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(e => e.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(e => e.SenderId);

                entity.HasOne(e => e.Receiver)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(e => e.ReceiverId);
            });

            // ================= CHAT =================
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ================= CHAT MESSAGES =================
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasOne(e => e.Chat)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(e => e.ChatId);

                entity.HasOne(e => e.Sender)
                    .WithMany()
                    .HasForeignKey(e => e.SenderId);
            });

            // ================= CHAT MEMBERS =================
            modelBuilder.Entity<ChatMember>(entity =>
            {
                entity.HasIndex(e => new { e.ChatId, e.UserId }).IsUnique();
            });
        }

    }
}

