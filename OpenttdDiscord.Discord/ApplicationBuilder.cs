using Microsoft.Extensions.Hosting;
using OpenttdDiscord.Discord;
using OpenttdDiscord.Infrastructure;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace OpenttdDiscord
{
    public static class ApplicationBuilder
    {
        public static IHostBuilder CreateHostBuilder()
        {
            return Host
                .CreateDefaultBuilder()
                .ConfigureLogging()
                .RegisterDependencies();
        }

        public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, configuration) =>
            {
                configuration.MinimumLevel.Verbose()
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();
            });
        }

        public static IHostBuilder RegisterDependencies(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.RegisterModules();
                services.AddHostedServices();
            });
        }

        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            return services
                .AddHostedService<DiscordService>();
        }
    }
}
