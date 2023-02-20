using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Ottd.Commands;
using OpenttdDiscord.Infrastructure.Ottd.Runners;

namespace OpenttdDiscord.Infrastructure.Ottd
{
    internal class OttdModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services
                .RegisterCommands()
                .RegisterRunners();
        }
    }

    internal static class OttdModuleExtensions
    {
        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            services.AddScoped<QueryServerRunner>();

            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, QueryServerCommand>();

            return services;
        }
    }
}
