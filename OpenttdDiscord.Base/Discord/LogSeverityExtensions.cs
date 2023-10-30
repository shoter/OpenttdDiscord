using System.Diagnostics.CodeAnalysis;
using Discord;
using Microsoft.Extensions.Logging;

namespace OpenttdDiscord.Base.Discord
{
    public static class LogSeverityExtensions
    {
        [ExcludeFromCodeCoverage]
        public static LogLevel ToLogLevel(this LogSeverity s)
        {
            return s switch
            {
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Verbose => LogLevel.Information,
                LogSeverity.Debug => LogLevel.Debug,
                _ => throw new Exception()
            };
        }
    }
}
