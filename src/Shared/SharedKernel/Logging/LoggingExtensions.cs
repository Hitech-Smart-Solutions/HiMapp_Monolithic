using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Himapp.SharedKernel.Logging
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Register basic logging providers and read minimal settings from configuration.
        /// For production, prefer Serilog (use UseSerilog in Program.cs) and OpenTelemetry.
        /// </summary>
        public static ILoggingBuilder AddSharedLogging(this ILoggingBuilder logging, IConfiguration configuration)
        {
            logging.AddConsole();
            logging.AddDebug();

            // TODO: add enrichment (e.g., module, environment) and integrate Serilog/OpenTelemetry when packages are added

            return logging;
        }
    }
}
