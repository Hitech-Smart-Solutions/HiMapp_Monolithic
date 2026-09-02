using Microsoft.AspNetCore.Http;

namespace Himapp.Api.src.Shared.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(
            string message = "The requested resource was not found.",
            string? errorCode = "NOT_FOUND")
            : base(
                message,
                StatusCodes.Status404NotFound,
                errorCode)
        {
        }
    }
}
