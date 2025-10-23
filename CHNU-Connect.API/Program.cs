using Microsoft.EntityFrameworkCore;
using CHNU_Connect.DAL.Data;
using CHNU_Connect.BLL;
using CHNU_Connect.API.Logging;
using CHNU_Connect.DAL.Extensions;
using Serilog;

namespace CHNU_Connect.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------- SERILOG CONFIGURATION ----------
            builder.ConfigureSerilog();

            // ---------- DB CONTEXT ----------
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ---------- BUSINESS LOGIC LAYER ----------
            builder.Services.AddBusinessLogic();
            builder.Services.AddDataAccessLayer(builder.Configuration);

            // ---------- JWT AUTHENTICATION ----------
            builder.Services.ConfigureJwt(builder.Configuration);

            // ---------- SWAGGER/OPENAPI ----------
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "CHNU Connect API",
                    Version = "v1",
                    Description = "API for CHNU Connect - University Social Network",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "CHNU Connect Team",
                        Email = "support@chnu.edu.ua"
                    }
                });

                // Add JWT Authorization support
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // ---------- CONTROLLERS ----------
            builder.Services.AddControllers();

            // ---------- CORS ----------
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // ---------- BUILD APP ----------
            var app = builder.Build();

            // ---------- ENSURE MIGRATIONS ----------
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
                SeedData.Initialize(db);
            }

            // ---------- MIDDLEWARE PIPELINE ----------
            app.UseMiddleware<CHNU_Connect.API.Middleware.GlobalExceptionHandlingMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CHNU Connect API v1");
                c.RoutePrefix = "swagger"; 
            });

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
