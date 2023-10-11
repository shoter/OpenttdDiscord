using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.Testing
{
    internal class TestingModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services
                .AddScoped<IStatusMonitorRepository, StatusMonitorRepository>()
                .RegisterCommandRunners()
                .RegisterCommands()
                .RegisterModals()
                .RegisterModalRunners();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal static class StatusesModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterCommandRunners(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection RegisterModals(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection RegisterModalRunners(this IServiceCollection services)
        {
            return services;
        }
    }
}