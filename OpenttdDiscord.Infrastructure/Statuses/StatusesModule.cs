using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Statuses.UseCases;

namespace OpenttdDiscord.Infrastructure.Statuses
{
    internal class StatusesModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services
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

            return services;
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {

            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {

            return services;
        }
    }

}
