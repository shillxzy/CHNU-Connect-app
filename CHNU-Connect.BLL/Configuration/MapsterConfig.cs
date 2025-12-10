using CHNU_Connect.BLL.DTOs.AdminAction;
using CHNU_Connect.BLL.DTOs.Comment;
using CHNU_Connect.BLL.DTOs.Event;
using CHNU_Connect.BLL.DTOs.EventParticipant;
using CHNU_Connect.BLL.DTOs.Group;
using CHNU_Connect.BLL.DTOs.GroupMember;
using CHNU_Connect.BLL.DTOs.Post;
using CHNU_Connect.BLL.DTOs.PostLike;
using CHNU_Connect.BLL.DTOs.User;
using CHNU_Connect.DAL.Entities;
using Mapster;

namespace CHNU_Connect.BLL.Configuration
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            // User mappings
            TypeAdapterConfig<User, UserDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.Username, src => src.FullName ?? src.Email)
                .Map(dest => dest.FullName, src => src.FullName)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.IsBlocked, src => src.IsBlocked);

            TypeAdapterConfig<CreateUserDto, User>
                .NewConfig()
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PasswordHash, src => src.Password)
                .Map(dest => dest.Role, src => src.Role)
                .Map(dest => dest.FullName, src => src.FullName)
                .Map(dest => dest.Faculty, src => src.Faculty)
                .Map(dest => dest.Course, src => src.Course)
                .Map(dest => dest.PhotoUrl, src => src.PhotoUrl)
                .Map(dest => dest.Bio, src => src.Bio);

            // Post mappings
            TypeAdapterConfig<Post, PostDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.AuthorId, src => src.UserId)
                .Map(dest => dest.Content, src => src.Content)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt);

            TypeAdapterConfig<CreatePostDto, Post>
                .NewConfig()
                .Map(dest => dest.UserId, src => src.AuthorId)
                .Map(dest => dest.Content, src => src.Content)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl);

            // Event mappings
            TypeAdapterConfig<Event, EventDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.StartTime, src => src.Date)
                .Map(dest => dest.EndTime, src => src.Date.AddHours(2)) 
                .Map(dest => dest.CreatedById, src => src.CreatorId);

            TypeAdapterConfig<CreateEventDto, Event>
                .NewConfig()
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Date, src => src.StartTime)
                .Map(dest => dest.CreatorId, src => src.CreatedById)
                .Map(dest => dest.IsPublic, src => src.IsPublic);

            // Group mappings
            TypeAdapterConfig<Group, GroupDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.CreatorId, src => src.CreatorId)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt);

            TypeAdapterConfig<CreateGroupDto, Group>
                .NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.CreatorId, src => src.CreatorId);

            // Comment mappings
            TypeAdapterConfig<Comment, CommentDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.PostId, src => src.PostId)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.Content, src => src.Content)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt);

            TypeAdapterConfig<CreateCommentDto, Comment>
                .NewConfig()
                .Map(dest => dest.PostId, src => src.PostId)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.Content, src => src.Content);

            // EventParticipant mappings
            TypeAdapterConfig<EventParticipant, EventParticipantDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.EventId, src => src.EventId)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.JoinedAt, src => src.JoinedAt);

            TypeAdapterConfig<CreateEventParticipantDto, EventParticipant>
                .NewConfig()
                .Map(dest => dest.EventId, src => src.EventId)
                .Map(dest => dest.UserId, src => src.UserId);

            // GroupMember mappings
            TypeAdapterConfig<GroupMember, GroupMemberDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.GroupId, src => src.GroupId)
                .Map(dest => dest.UserId, src => src.UserId);

            TypeAdapterConfig<CreateGroupMemberDto, GroupMember>
                .NewConfig()
                .Map(dest => dest.GroupId, src => src.GroupId)
                .Map(dest => dest.UserId, src => src.UserId);

            // PostLike mappings
            TypeAdapterConfig<PostLike, PostLikeDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.PostId, src => src.PostId)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt);

            TypeAdapterConfig<CreatePostLikeDto, PostLike>
                .NewConfig()
                .Map(dest => dest.PostId, src => src.PostId)
                .Map(dest => dest.UserId, src => src.UserId);

            // AdminAction mappings
            TypeAdapterConfig<AdminAction, AdminActionDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.AdminId, src => src.AdminId)
                .Map(dest => dest.TargetId, src => src.TargetUserId)
                .Map(dest => dest.ActionType, src => src.Action)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt);

            TypeAdapterConfig<CreateAdminActionDto, AdminAction>
                .NewConfig()
                .Map(dest => dest.AdminId, src => src.AdminId)
                .Map(dest => dest.TargetUserId, src => src.TargetId)
                .Map(dest => dest.Action, src => src.Action)
                .Map(dest => dest.Reason, src => src.Reason);
        }
    }
}
