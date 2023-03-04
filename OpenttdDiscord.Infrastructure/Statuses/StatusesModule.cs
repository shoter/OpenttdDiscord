using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
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

#pragma warning disable SA1402 // File may only contain a single type
    internal static class StatusesModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterUseCases(this IServiceCollection services)
        {
            services.AddScoped<IRegisterStatusMonitorUseCase, RegisterStatusMonitorUseCase>();
            services.AddScoped<IGetStatusMonitorsForServerUseCase, GetStatusMonitorsForServerUseCase>();
            services.AddScoped<IRemoveStatusMonitorUseCase, RemoveStatusMonitorUseCase>();
            services.AddScoped<IUpdateStatusMonitorUseCase, UpdateStatusMonitorUseCase>();
            services.AddScoped<ICheckIfStatusMonitorExistsUseCase, CheckIfStatusMonitorExistsUseCase>();
            return services;
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            services.AddScoped<RegisterStatusMonitorRunner>();
            services.AddScoped<RemoveStatusMonitorRunner>();
            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, RegisterStatusMonitorCommand>();
            services.AddSingleton<IOttdSlashCommand, RemoveStatusMonitorCommand>();
            return services;
        }
    }
}
