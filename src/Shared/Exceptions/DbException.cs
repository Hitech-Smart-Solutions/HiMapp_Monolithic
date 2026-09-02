namespace Himapp.Api.src.Shared.Exceptions
{
    public class DbException : AppException
    {
        public DbException(
            string message = "A database error occurred.",
            string? errorCode = "DATABASE_ERROR")
            : base(
                message,
                StatusCodes.Status500InternalServerError,
                errorCode)
        {
        }
    }
}
