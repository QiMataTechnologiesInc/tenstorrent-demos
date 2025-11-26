using System;
using Microsoft.Extensions.Logging;

namespace Tenstorrent.Shared
{
    public static class Logging
    {
        private const string LevelEnv = "TT_LOG_LEVEL";

        public static LogLevel ResolveLogLevel(string? overrideLevel = null)
        {
            var candidate = string.IsNullOrWhiteSpace(overrideLevel)
                ? Environment.GetEnvironmentVariable(LevelEnv)
                : overrideLevel;

            if (string.IsNullOrWhiteSpace(candidate))
            {
                return LogLevel.Information;
            }

            return candidate.Trim().ToUpperInvariant() switch
            {
                "TRACE" => LogLevel.Trace,
                "DEBUG" => LogLevel.Debug,
                "INFO" or "INFORMATION" => LogLevel.Information,
                "WARN" or "WARNING" => LogLevel.Warning,
                "ERROR" => LogLevel.Error,
                "CRITICAL" or "FATAL" => LogLevel.Critical,
                _ => LogLevel.Information,
            };
        }

        public static ILoggerFactory CreateLoggerFactory(LogLevel? minimumLevel = null)
        {
            var resolved = minimumLevel ?? ResolveLogLevel();

            return LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(resolved);
                builder.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                });
            });
        }
    }
}
