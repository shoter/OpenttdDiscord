using System.Net.Security;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Domain.Chatting;
using OpenttdDiscord.Domain.Chatting.Translating;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.EventLogs.UseCases;
using OpenttdDiscord.Infrastructure.Chatting.Commands;
using OpenttdDiscord.Infrastructure.Chatting.Runners;
using OpenttdDiscord.Infrastructure.Chatting.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.EventLogs.Commands;
using OpenttdDiscord.Infrastructure.EventLogs.Runners;
using OpenttdDiscord.Infrastructure.EventLogs.UseCases;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.Chatting
{
    internal class ChattingModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IChatChannelRepository, ChatChannelRepository>();
            services.AddSingleton<IChatTranslator, ChatTranslator>();
            services.AddSingleton<IEmojiTranslator, EmojiTranslator>();
            services
                .RegisterUseCases()
                .RegisterCommands()
                .RegisterRunners();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal static class ChattingModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
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
            services.AddScoped<UnregisterChatChannelRunner>();

            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, RegisterChatChannelCommand>();
            services.AddSingleton<IOttdSlashCommand, UnregisterChatChannelCommand>();

            return services;
        }
    }
}
