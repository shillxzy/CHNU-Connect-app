using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using CHNU_Connect.BLL.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CHNU_Connect.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError; // default 500
            var message = "Unexpected error occurred.";

            switch (exception)
            {
                // 🔹 Authentication & Authorization
                case UserNotFoundException: statusCode = HttpStatusCode.NotFound; message = exception.Message; break;
                case InvalidCredentialsException: statusCode = HttpStatusCode.Unauthorized; message = exception.Message; break;
                case TokenExpiredException: statusCode = HttpStatusCode.Unauthorized; message = exception.Message; break;
                case EmailAlreadyUsedException: statusCode = HttpStatusCode.BadRequest; message = exception.Message; break;

                // 🧱 User input / Validation
                case PasswordTooWeakException: statusCode = HttpStatusCode.BadRequest; message = exception.Message; break;
                case InvalidEmailFormatException: statusCode = HttpStatusCode.BadRequest; message = exception.Message; break;

                // 🗃️ Database / Persistence
                case DatabaseConnectionException: statusCode = HttpStatusCode.InternalServerError; message = exception.Message; break;
                case DataIntegrityViolationException: statusCode = HttpStatusCode.Conflict; message = exception.Message; break;
                case EntityNotFoundException: statusCode = HttpStatusCode.NotFound; message = exception.Message; break;
                case DuplicateKeyException: statusCode = HttpStatusCode.Conflict; message = exception.Message; break;

                // 💬 Domain / Business logic
                case PostNotFoundException: statusCode = HttpStatusCode.NotFound; message = exception.Message; break;
                case CommentNotFoundException: statusCode = HttpStatusCode.NotFound; message = exception.Message; break;
                case UserBlockedException: statusCode = HttpStatusCode.Forbidden; message = exception.Message; break;
               
                case GroupNotFoundException: statusCode = HttpStatusCode.NotFound; message = exception.Message; break;

                // 🌐 System / Server
                case FileUploadException: statusCode = HttpStatusCode.InternalServerError; message = exception.Message; break;
                case ExternalApiException: statusCode = HttpStatusCode.BadGateway; message = exception.Message; break;
                case InternalServerErrorException: statusCode = HttpStatusCode.InternalServerError; message = exception.Message; break;

                default: break;
            }

            response.StatusCode = (int)statusCode;
            var result = JsonSerializer.Serialize(new { error = message });
            return response.WriteAsync(result);
        }
    }
}
