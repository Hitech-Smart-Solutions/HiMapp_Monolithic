using Himapp.Api.src.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

            // Find deepest/root exception
            var rootException = exception;

            while (rootException.InnerException != null)
            {
                rootException = rootException.InnerException;
            }

            // =========================================================
            // Build complete exception chain
            // =========================================================

            var exceptionChain = new List<string>();

            var currentException = exception;
            var level = 0;

            while (currentException != null)
            {
                exceptionChain.Add($"""
                    ===== Exception Level {level} =====
                    Type: {currentException.GetType().FullName}
                    Message: {currentException.Message}
                    StackTrace:
                    {currentException.StackTrace}
                    """);

                currentException = currentException.InnerException;
                level++;
            }

            var detailedException = string.Join(
                Environment.NewLine + Environment.NewLine,
                exceptionChain
            );

            // =========================================================
            // General exception logging
            // =========================================================

            _logger.LogError(
                exception,
                """
                ==================== UNHANDLED EXCEPTION ====================

                TraceId:
                {TraceId}

                Request:
                {Method} {Path}

                Exception Type:
                {ExceptionType}

                Exception Message:
                {ExceptionMessage}

                Root Exception Type:
                {RootExceptionType}

                Root Exception Message:
                {RootExceptionMessage}

                FULL EXCEPTION CHAIN:
                {DetailedException}

                =============================================================
                """,
                traceId,
                context.Request.Method,
                context.Request.Path,
                exception.GetType().FullName,
                exception.Message,
                rootException.GetType().FullName,
                rootException.Message,
                detailedException
            );

            // =========================================================
            // Special PostgreSQL logging
            // =========================================================

            var postgresException =
                exception.GetBaseException() as PostgresException;

            if (postgresException != null)
            {
                _logger.LogError(
                    """
                    ==================== POSTGRESQL ERROR ====================

                    TraceId:
                    {TraceId}

                    Severity:
                    {Severity}

                    SqlState:
                    {SqlState}

                    Message:
                    {Message}

                    Detail:
                    {Detail}

                    Hint:
                    {Hint}

                    Table:
                    {Table}

                    Column:
                    {Column}

                    Constraint:
                    {Constraint}

                    Where:
                    {Where}

                    ==========================================================
                    """,
                    traceId,
                    postgresException.Severity,
                    postgresException.SqlState,
                    postgresException.MessageText,
                    postgresException.Detail,
                    postgresException.Hint,
                    postgresException.TableName,
                    postgresException.ColumnName,
                    postgresException.ConstraintName,
                    postgresException.Where
                );
            }

            // =========================================================
            // Response
            // =========================================================

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

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}