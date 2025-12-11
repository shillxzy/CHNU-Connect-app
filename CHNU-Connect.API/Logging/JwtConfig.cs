using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CHNU_Connect.API.Logging
{
    public static class JwtConfig
    {
        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings"); //
            var secretKey = jwtSettings["SecretKey"] ?? "CHNU_Connect_Super_Secret_Key_2024_For_JWT_Token_Generation";
            var issuer = jwtSettings["Issuer"] ?? "CHNU-Connect";
            var audience = jwtSettings["Audience"] ?? "CHNU-Connect-Users";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();
        }

        public static string GenerateJwtToken(string userId, string email, string role, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "CHNU_Connect_Super_Secret_Key_2024_For_JWT_Token_Generation";
            var issuer = jwtSettings["Issuer"] ?? "CHNU-Connect";
            var audience = jwtSettings["Audience"] ?? "CHNU-Connect-Users";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new System.Security.Claims.Claim("userId", userId),
                new System.Security.Claims.Claim("email", email),
                new System.Security.Claims.Claim("role", role),
                new System.Security.Claims.Claim("jti", Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
