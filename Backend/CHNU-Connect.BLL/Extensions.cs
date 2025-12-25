using CHNU_Connect.BLL.Services;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CHNU_Connect.BLL
{
    public static class Extensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IEventParticipantService, EventParticipantService>();
            services.AddScoped<IGroupMemberService, GroupMemberService>();
            services.AddScoped<IPostLikeService, PostLikeService>();
            services.AddScoped<IAdminActionService, AdminActionService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IChatService, ChatService>();
            return services;
        }
    }
}
