using Himapp.Api.src.Shared.Exceptions;
using System.Text.Json;

namespace Himapp.Api.src.Shared.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
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
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var traceId = context.TraceIdentifier;

            // Get the deepest/actual exception
            var rootException = exception;
            while (rootException.InnerException != null)
            {
                rootException = rootException.InnerException;
            }

            // Log complete exception details
            _logger.LogError(
                exception,
                """
                Unhandled Exception

                TraceId: {TraceId}
                Request: {Method} {Path}
                Exception Type: {ExceptionType}
                Exception Message: {ExceptionMessage}
                Root Exception Type: {RootExceptionType}
                Root Exception Message: {RootExceptionMessage}
                Stack Trace:
                {StackTrace}
                """,
                traceId,
                context.Request.Method,
                context.Request.Path,
                exception.GetType().FullName,
                exception.Message,
                rootException.GetType().FullName,
                rootException.Message,
                exception.ToString()
            );

            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "An unexpected error occurred.";
            var errorCode = "INTERNAL_SERVER_ERROR";

            if (exception is AppException appException)
            {
                statusCode = appException.StatusCode;
                message = appException.Message;
                errorCode = appException.ErrorCode ?? "APPLICATION_ERROR";
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message,
                errorCode,
                traceId
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
