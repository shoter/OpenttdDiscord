using Akka.TestKit.Xunit2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Tests.Common.Xunits;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests;

public abstract class BaseActorTestKit : TestKit
{
    protected IServiceProvider Sp { get; }
    public BaseActorTestKit(ITestOutputHelper outputHelper)
    {
        ServiceCollection services = new();
        services.AddLogging(
            logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddProvider(new XUnitLoggerProvider(outputHelper));
            });

        InitializeServiceProvider(services);

        Sp = services.BuildServiceProvider();
    }

    protected virtual void InitializeServiceProvider(IServiceCollection services)
    {
    }
}