using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Maintenance.Commands;
using OpenttdDiscord.Infrastructure.Maintenance.HealthChecks;
using OpenttdDiscord.Infrastructure.Maintenance.Runners;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.Maintenance
{
    public class MaintenanceModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthcheck>("Db");
            services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
            services.Configure<HealthCheckPublisherOptions>(
                options =>
                {
                    options.Delay = TimeSpan.FromSeconds(5);
                    options.Period = TimeSpan.FromSeconds(20);
                });

            services
                .RegisterRunners()
                .RegisterCommands();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal static class OttdModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            services.AddScoped<HealthCheckRunner>();

            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, HealthCheckCommand>();

            return services;
        }
    }
}