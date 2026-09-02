using Microsoft.AspNetCore.Http;

namespace Himapp.Api.src.Shared.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(
            string message = "You are not authorized to perform this action.",
            string? errorCode = "UNAUTHORIZED")
            : base(
                message,
                StatusCodes.Status401Unauthorized,
                errorCode)
        {
        }
    }
}
