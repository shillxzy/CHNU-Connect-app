using CHNU_Connect.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

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
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventParticipant> EventParticipants { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<AdminAction> AdminActions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
                entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(30).IsRequired();
                entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(255);
                entity.Property(e => e.Faculty).HasColumnName("faculty").HasMaxLength(255);
                entity.Property(e => e.Course).HasColumnName("course");
                entity.Property(e => e.PhotoUrl).HasColumnName("photo_url");
                entity.Property(e => e.Bio).HasColumnName("bio");
                entity.Property(e => e.IsBlocked).HasColumnName("is_blocked").HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            });

            // Posts
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("posts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.HasOne(e => e.User).WithMany(u => u.Posts).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Content).HasColumnName("content").IsRequired();
                entity.Property(e => e.ImageUrl).HasColumnName("image_url");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            // PostLikes
            modelBuilder.Entity<PostLike>(entity =>
            {
                entity.ToTable("post_likes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PostId).HasColumnName("post_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
                entity.HasOne(e => e.Post).WithMany(p => p.Likes).HasForeignKey(e => e.PostId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.PostId, e.UserId }).IsUnique();
            });

            // Comments
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PostId).HasColumnName("post_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Content).HasColumnName("content").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
                entity.HasOne(e => e.Post).WithMany(p => p.Comments).HasForeignKey(e => e.PostId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            // Groups
            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("groups");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CreatorId).HasColumnName("creator_id");
                entity.HasOne(e => e.Creator).WithMany().HasForeignKey(e => e.CreatorId).OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.IsPublic).HasColumnName("is_public").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            });

            // GroupMembers
            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.ToTable("group_members");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.GroupId).HasColumnName("group_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Role).HasColumnName("role").HasDefaultValue("member");
                entity.Property(e => e.JoinedAt).HasColumnName("joined_at").HasDefaultValueSql("now()");
                entity.HasOne(e => e.Group).WithMany(g => g.Members).HasForeignKey(e => e.GroupId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.GroupId, e.UserId }).IsUnique();
            });

            // Events
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("events");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Date).HasColumnName("date").IsRequired();
                entity.Property(e => e.CreatorId).HasColumnName("creator_id");
                entity.HasOne(e => e.Creator).WithMany().HasForeignKey(e => e.CreatorId).OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.IsPublic).HasColumnName("is_public").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            });

            // EventParticipants
            modelBuilder.Entity<EventParticipant>(entity =>
            {
                entity.ToTable("event_participants");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EventId).HasColumnName("event_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.JoinedAt).HasColumnName("joined_at").HasDefaultValueSql("now()");
                entity.HasOne(e => e.Event).WithMany(e => e.Participants).HasForeignKey(e => e.EventId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.EventId, e.UserId }).IsUnique();
            });

            // Messages
            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("messages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SenderId).HasColumnName("sender_id");
                entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
                entity.Property(e => e.Content).HasColumnName("content").IsRequired();
                entity.Property(e => e.IsRead).HasColumnName("is_read").HasDefaultValue(false);
                entity.Property(e => e.SentAt).HasColumnName("sent_at").HasDefaultValueSql("now()");
                entity.HasOne(e => e.Sender).WithMany(u => u.SentMessages).HasForeignKey(e => e.SenderId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Receiver).WithMany(u => u.ReceivedMessages).HasForeignKey(e => e.ReceiverId).OnDelete(DeleteBehavior.Cascade);
            });

            // AdminActions
            modelBuilder.Entity<AdminAction>(entity =>
            {
                entity.ToTable("admin_actions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AdminId).HasColumnName("admin_id");
                entity.Property(e => e.TargetUserId).HasColumnName("target_user_id");
                entity.Property(e => e.Action).HasColumnName("action").HasMaxLength(50);
                entity.Property(e => e.Reason).HasColumnName("reason");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
                entity.HasOne(e => e.Admin).WithMany().HasForeignKey(e => e.AdminId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.TargetUser).WithMany().HasForeignKey(e => e.TargetUserId).OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
