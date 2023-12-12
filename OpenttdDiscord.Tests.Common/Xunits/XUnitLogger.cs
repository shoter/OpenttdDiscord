using System.Text;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Basics;
using Xunit.Abstractions;

namespace OpenttdDiscord.Tests.Common.Xunits
{
    public class XUnitLogger : ILogger
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly string className;
        private readonly string loggerName;
        private readonly LoggerExternalScopeProvider scopeProvider;

        public static ILogger CreateLogger(ITestOutputHelper testOutputHelper) => new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), string.Empty);

        public static ILogger<T> CreateLogger<T>(ITestOutputHelper testOutputHelper) => new XUnitLogger<T>(testOutputHelper, new LoggerExternalScopeProvider());

        public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider, string categoryName, string loggerName = "")
        {
            this.testOutputHelper = testOutputHelper;
            this.scopeProvider = scopeProvider;
            this.className = categoryName.Split(".").Last();
            this.loggerName = loggerName;
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
            => scopeProvider.Push(state);

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            try
            {
                var sb = new StringBuilder();
                sb
                  .Append("[").Append(DateTime.Now.ToTime()).Append("]")
                  .Append("[").Append(GetLogLevelString(logLevel).ToUpperInvariant()).Append("]")
                  .Append(" <").Append(className).Append("> ");
                AppendLoggerName<TState>(sb);

                sb.Append(formatter(state, exception));

                if (exception != null)
                {
                    sb.Append('\n').Append(exception);
                }

                AppendScopes<TState>(sb);
                Output<TState>(sb);
            }
            catch (Exception ex)
            {
                try
                {
                    Console.WriteLine(ex);
                }
                catch
                {
                }
            }
        }

        private void Output<TState>(StringBuilder sb)
        {
            string log = sb.ToString();
            testOutputHelper.WriteLine(log);
            Console.WriteLine(log);
        }

        private void AppendScopes<TState>(StringBuilder sb)
        {
            scopeProvider.ForEachScope(
                (
                    scope,
                    state) =>
                {
                    state.Append("\n => ");
                    state.Append(scope);
                }, sb);
        }

        private void AppendLoggerName<TState>(StringBuilder sb)
        {
            if (loggerName != string.Empty)
            {
                sb.Append(" {");
                sb.Append(loggerName);
                sb.Append("} ");
            }
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "fail",
                LogLevel.Critical => "crit",
                _ => "unkn",
            };
        }
    }
}