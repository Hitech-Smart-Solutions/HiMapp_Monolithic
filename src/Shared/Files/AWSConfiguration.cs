using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace Himapp.Shared.Files
{
    /// <summary>
    /// Centralized AWS configuration and credentials management.
    /// Initialize this class at application startup to make AWS settings available across all modules.
    /// </summary>
    public static class AWSConfiguration
    {
        public static string? RootFolder { get; private set; }
        public static string? BucketName { get; private set; }
        public static string? AwsToken { get; private set; }
        public static string? AwsKey { get; private set; }
        public static AWSCredentials? AWSCredentials { get; private set; }

        /// <summary>
        /// Initialize AWS configuration from IConfiguration.
        /// Call this method once at application startup.
        /// </summary>
        public static void Initialize(IConfiguration configuration)
        {
            RootFolder = configuration["RootFolder"];
            BucketName = configuration["BucketName"];
            AwsToken = configuration["AWS_SECRET_ACCESS_KEY"];
            AwsKey = configuration["AWS_ACCESS_KEY_ID"];

            AWSCredentials = new BasicAWSCredentials(AwsKey, AwsToken);
        }
    }
}
