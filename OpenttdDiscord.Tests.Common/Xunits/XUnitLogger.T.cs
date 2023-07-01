using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace OpenttdDiscord.Tests.Common.Xunits
{
    public class XUnitLogger<T> : XUnitLogger, ILogger<T>
    {
        public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider, string loggerName = "")
            : base(testOutputHelper, scopeProvider, typeof(T).FullName!, loggerName)
        {
        }
    }
}