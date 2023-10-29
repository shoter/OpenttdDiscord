using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.AutoReply
{
    public class AutoReplyModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services
                .RegisterUseCases()
                .RegisterCommands()
                .RegisterRunners();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal static class AutoReplyModuleExtensions
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