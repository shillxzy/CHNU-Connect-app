using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CHNU_Connect.DAL
{
    public static class Extensions
    {
        public static void ConfigureEntityFramework(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<Data.AppDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<UOW.IUnitOfWork, UOW.UnitOfWork>();
            
            // Реєстрація специфічних репозиторіїв
            services.AddScoped<Repositories.Interfaces.IUserRepository, Repositories.UserRepository>();
            services.AddScoped<Repositories.Interfaces.IPostRepository, Repositories.PostRepository>();
            services.AddScoped<Repositories.Interfaces.IEventRepository, Repositories.EventRepository>();
            services.AddScoped<Repositories.Interfaces.IGroupRepository, Repositories.GroupRepository>();
            services.AddScoped<Repositories.Interfaces.IMessageRepository, Repositories.MessageRepository>();
            services.AddScoped<Repositories.Interfaces.IAdminActionRepository, Repositories.AdminActionRepository>();
            services.AddScoped<Repositories.Interfaces.ICommentRepository, Repositories.CommentRepository>();
            services.AddScoped<Repositories.Interfaces.IEventParticipantRepository, Repositories.EventParticipantRepository>();
            services.AddScoped<Repositories.Interfaces.IGroupMemberRepository, Repositories.GroupMemberRepository>();
            services.AddScoped<Repositories.Interfaces.IPostLikeRepository, Repositories.PostLikeRepository>();
        }
    }
}
