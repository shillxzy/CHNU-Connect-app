using CHNU_Connect.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CHNU_Connect.DAL.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Users.Any())
                return; // вже є дані

            // ---------- USERS ----------
            var users = new List<User>();
            for (int i = 1; i <= 10; i++)
            {
                users.Add(new User
                {
                    Email = $"user{i}@chnu.edu.ua",
                    PasswordHash = $"hashed_password{i}",
                    Role = i % 4 == 0 ? "admin" : i % 3 == 0 ? "teacher" : "student",
                    FullName = $"User {i}",
                    Faculty = $"Faculty {i % 3 + 1}",
                    Course = (i % 4) + 1,
                    PhotoUrl = $"https://picsum.photos/seed/user{i}/100",
                    Bio = $"Hello, I am user {i}"
                });
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            // ---------- POSTS ----------
            var posts = new List<Post>();
            for (int i = 1; i <= 10; i++)
            {
                posts.Add(new Post
                {
                    UserId = users[i % 10].Id,
                    Content = $"This is post {i}",
                    ImageUrl = i % 2 == 0 ? $"https://picsum.photos/seed/post{i}/200" : null
                });
            }
            context.Posts.AddRange(posts);
            context.SaveChanges();

            // ---------- COMMENTS ----------
            var comments = new List<Comment>();
            for (int i = 1; i <= 10; i++)
            {
                comments.Add(new Comment
                {
                    PostId = posts[i % 10].Id,
                    UserId = users[(i + 1) % 10].Id,
                    Content = $"Comment {i} on post {posts[i % 10].Id}"
                });
            }
            context.Comments.AddRange(comments);
            context.SaveChanges();

            // ---------- POST LIKES ----------
            var postLikes = new List<PostLike>();
            for (int i = 1; i <= 10; i++)
            {
                postLikes.Add(new PostLike
                {
                    PostId = posts[i % 10].Id,
                    UserId = users[(i + 2) % 10].Id
                });
            }
            context.PostLikes.AddRange(postLikes);
            context.SaveChanges();

            // ---------- GROUPS ----------
            var groups = new List<Group>();
            for (int i = 1; i <= 10; i++)
            {
                groups.Add(new Group
                {
                    Name = $"Group {i}",
                    Description = $"Description for group {i}",
                    CreatorId = users[i % 10].Id,
                    IsPublic = i % 2 == 0
                });
            }
            context.Groups.AddRange(groups);
            context.SaveChanges();

            // ---------- GROUP MEMBERS ----------
            var groupMembers = new List<GroupMember>();
            foreach (var g in groups)
            {
                for (int i = 0; i < 10; i++)
                {
                    groupMembers.Add(new GroupMember
                    {
                        GroupId = g.Id,
                        UserId = users[i].Id,
                        Role = i == 0 ? "admin" : "member"
                    });
                }
            }
            context.GroupMembers.AddRange(groupMembers);
            context.SaveChanges();

            // ---------- EVENTS ----------
            var events = new List<Event>();
            for (int i = 1; i <= 10; i++)
            {
                events.Add(new Event
                {
                    Title = $"Event {i}",
                    Description = $"Event {i} description",
                    Date = DateTime.UtcNow.AddDays(i),
                    CreatorId = users[i % 10].Id,
                    IsPublic = i % 2 == 0
                });
            }
            context.Events.AddRange(events);
            context.SaveChanges();

            // ---------- EVENT PARTICIPANTS ----------
            var eventParticipants = new List<EventParticipant>();
            foreach (var e in events)
            {
                for (int i = 0; i < 10; i++)
                {
                    eventParticipants.Add(new EventParticipant
                    {
                        EventId = e.Id,
                        UserId = users[i].Id
                    });
                }
            }
            context.EventParticipants.AddRange(eventParticipants);
            context.SaveChanges();

            // ---------- MESSAGES ----------
            var messages = new List<Message>();
            for (int i = 1; i <= 10; i++)
            {
                messages.Add(new Message
                {
                    SenderId = users[i % 10].Id,
                    ReceiverId = users[(i + 1) % 10].Id,
                    Content = $"Message {i} from user {users[i % 10].Id} to user {users[(i + 1) % 10].Id}"
                });
            }
            context.Messages.AddRange(messages);
            context.SaveChanges();

            // ---------- ADMIN ACTIONS ----------
            var adminActions = new List<AdminAction>();
            var admins = users.Where(u => u.Role == "admin").ToList();
            for (int i = 1; i <= 10; i++)
            {
                adminActions.Add(new AdminAction
                {
                    AdminId = admins[i % admins.Count].Id,
                    TargetUserId = users[(i + 3) % 10].Id,
                    Action = i % 2 == 0 ? "block" : "unblock",
                    Reason = $"Action reason {i}"
                });
            }
            context.AdminActions.AddRange(adminActions);
            context.SaveChanges();
        }
    }
}
