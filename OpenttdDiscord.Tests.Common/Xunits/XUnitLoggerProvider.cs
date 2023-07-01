using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace OpenttdDiscord.Tests.Common.Xunits
{
    public class XUnitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly LoggerExternalScopeProvider scopeProvider = new LoggerExternalScopeProvider();
        private readonly string loggerName;

        public XUnitLoggerProvider(ITestOutputHelper testOutputHelper, string loggerName = "")
        {
            this.testOutputHelper = testOutputHelper;
            this.loggerName = loggerName;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XUnitLogger(testOutputHelper, scopeProvider, categoryName, loggerName);
        }

        public void Dispose()
        {
        }
    }
}