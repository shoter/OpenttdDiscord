using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Domain.Chatting;
using OpenttdDiscord.Domain.Chatting.Translating;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Infrastructure.Chatting.Commands;
using OpenttdDiscord.Infrastructure.Chatting.Runners;
using OpenttdDiscord.Infrastructure.Chatting.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
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