using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Statuses.Commands;
using OpenttdDiscord.Infrastructure.Statuses.Runners;
using OpenttdDiscord.Infrastructure.Statuses.UseCases;

namespace OpenttdDiscord.Infrastructure.Statuses
{
    internal class StatusesModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services
                .AddScoped<IStatusMonitorRepository, StatusMonitorRepository>()
                .RegisterUseCases()
                .RegisterRunners()
                .RegisterCommands();
        }
    }

    internal static class StatusesModuleExtensions
    {

        public static IServiceCollection RegisterUseCases(this IServiceCollection services)
        {
            services.AddScoped<IRegisterStatusMonitorUseCase, RegisterStatusMonitorUseCase>();
            services.AddScoped<IGetStatusMonitorsForServerUseCase, GetStatusMonitorsForServerUseCase>();
            return services;
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            services.AddScoped<RegisterStatusMonitorRunner>();
            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, RegisterStatusMonitorCommand>();
            return services;
        }
    }

}
