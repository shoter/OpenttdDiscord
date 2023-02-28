using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Infrastructure.Chatting.Commands;
using OpenttdDiscord.Infrastructure.Chatting.Runners;
using OpenttdDiscord.Infrastructure.Chatting.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Modularity;
using System.Net.Security;

namespace OpenttdDiscord.Infrastructure.Chatting
{
    internal class ChattingModule : IModule
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

    internal static class ChattingModuleExtensions
    {
        public static IServiceCollection RegisterUseCases(this IServiceCollection services)
        {
            services.AddScoped<IRegisterChatChannelUseCase, RegisterChatChannelUseCase>();
            services.AddScoped<IGetChatChannelUseCase, GetChatChannelUseCase>();
            services.AddScoped<IUnregisterChatChannelUseCase, UnregisterChatChannelUseCase>();

            return services;
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            services.AddScoped<RegisterChatChannelRunner>();

            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, RegisterChatChannelCommand>();

            return services;
        }
    }
}
