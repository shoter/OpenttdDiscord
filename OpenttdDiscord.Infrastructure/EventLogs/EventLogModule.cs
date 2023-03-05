using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.EventLogs
{
    internal class EventLogModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IChatChannelRepository, ChatChannelRepository>();
            services
                .RegisterUseCases()
                .RegisterCommands()
                .RegisterRunners();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal static class EventLogModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterUseCases(this IServiceCollection services)
        {
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
