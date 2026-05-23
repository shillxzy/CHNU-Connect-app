using CHNU_Connect.DAL.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Text.Json;

namespace CHNU_Connect.API.Middleware
{
    public class BanCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public BanCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                {
                    using var scope = context.RequestServices.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var isBlocked = await db.Users
                        .Where(u => u.Id == userId)
                        .Select(u => u.IsBlocked)
                        .FirstOrDefaultAsync();

                    if (isBlocked)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonSerializer.Serialize(new { reason = "banned" }));
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
