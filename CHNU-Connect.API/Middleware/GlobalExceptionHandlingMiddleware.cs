using System.Net;
using System.Text.Json;
using CHNU_Connect.BLL.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CHNU_Connect.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError; // default 500
            string message = "Unexpected error occurred.";

            switch (exception)
            {
                // 🔹 Authentication & Authorization
                case UserNotFoundException:
                    statusCode = HttpStatusCode.NotFound; message = exception.Message; break;
                case InvalidCredentialsException:
                case TokenExpiredException:
                    statusCode = HttpStatusCode.Unauthorized; message = exception.Message; break;
                case EmailAlreadyUsedException:
                    statusCode = HttpStatusCode.BadRequest; message = exception.Message; break;

                // 🧱 User input / Validation
                case PasswordTooWeakException:
                case InvalidEmailFormatException:
                    statusCode = HttpStatusCode.BadRequest; message = exception.Message; break;

                // 🗃️ Database / Persistence
                case DatabaseConnectionException:
                    statusCode = HttpStatusCode.InternalServerError; message = exception.Message; break;
                case DataIntegrityViolationException:
                case DuplicateKeyException:
                    statusCode = HttpStatusCode.Conflict; message = exception.Message; break;
                case EntityNotFoundException:
                    statusCode = HttpStatusCode.NotFound; message = exception.Message; break;

                // 💬 Domain / Business logic
                case PostNotFoundException:
                case CommentNotFoundException:
                case GroupNotFoundException:
                    statusCode = HttpStatusCode.NotFound; message = exception.Message; break;
                case UserBlockedException:
                    statusCode = HttpStatusCode.Forbidden; message = exception.Message; break;

                // 🌐 System / Server
                case FileUploadException:
                case InternalServerErrorException:
                    statusCode = HttpStatusCode.InternalServerError; message = exception.Message; break;
                case ExternalApiException:
                    statusCode = HttpStatusCode.BadGateway; message = exception.Message; break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = exception.Message;
                    break;
            }

            // 🔹 Логування винятку з повним стектрейсом
            _logger.LogError(exception, "Exception caught in GlobalExceptionHandlingMiddleware. HTTP {StatusCode}: {Message}", (int)statusCode, message);

            response.StatusCode = (int)statusCode;
            var result = JsonSerializer.Serialize(new { error = message });
            await response.WriteAsync(result);
        }
    }
}
