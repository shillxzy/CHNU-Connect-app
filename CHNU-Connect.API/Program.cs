using Microsoft.EntityFrameworkCore;
using CHNU_Connect.DAL.Data;

namespace CHNU_Connect.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------- DB CONTEXT ----------
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ---------- BUILD APP ----------
            var app = builder.Build();

            // ---------- ENSURE MIGRATIONS ----------
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();  // застосовує міграції
                SeedData.Initialize(db); // сідування даних
            }


            app.UseMiddleware<CHNU_Connect.API.Middleware.GlobalExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();

            // app.UseAuthorization();
            // app.MapControllers();

            app.Run();
        }
    }
}
