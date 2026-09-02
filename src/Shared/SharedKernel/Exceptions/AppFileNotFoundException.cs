using Microsoft.AspNetCore.Http;

namespace Himapp.Api.src.Shared.Exceptions
{
    public class AppFileNotFoundException : AppException
    {
        public AppFileNotFoundException(
            string message = "The requested file was not found.",
            string? errorCode = "FILE_NOT_FOUND")
            : base(
                message,
                StatusCodes.Status404NotFound,
                errorCode)
        {
        }
    }
}
