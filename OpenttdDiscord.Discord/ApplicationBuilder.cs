using Microsoft.Extensions.Hosting;
using OpenttdDiscord.Discord;
using OpenttdDiscord.Infrastructure;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OpenttdDiscord.Discord.Options;

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
            return hostBuilder.ConfigureServices((HostBuilderContext context, IServiceCollection services) =>
            {
                services
                    .RegisterModules()
                    .AddHostedServices()
                    .ConfigureOptions(context);
            });
        }

        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            return services
                .AddHostedService<DiscordService>();
        }

        public static IServiceCollection ConfigureOptions(this IServiceCollection services, HostBuilderContext context)
        {
            return services
                .Configure<DiscordOptions>(context.Configuration.GetSection("SubscriptionOptions"));

        }
    }
}
