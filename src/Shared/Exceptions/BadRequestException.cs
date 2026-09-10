namespace Himapp.Api.src.Shared.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(
            string message = "The request is invalid.",
            string? errorCode = "BAD_REQUEST")
            : base(
                message,
                StatusCodes.Status400BadRequest,
                errorCode)
        {
        }
    }
}
