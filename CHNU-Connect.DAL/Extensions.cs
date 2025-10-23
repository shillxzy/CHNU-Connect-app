using Microsoft.EntityFrameworkCore;

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
        }
    }
}
