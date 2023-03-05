using System.Net.Security;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Infrastructure.Chatting.Commands;
using OpenttdDiscord.Infrastructure.Chatting.Runners;
using OpenttdDiscord.Infrastructure.Chatting.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Modularity;

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

#pragma warning disable SA1402 // File may only contain a single type
    internal static class ChattingModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterUseCases(this IServiceCollection services)
        {
            services.AddScoped<IRegisterChatChannelUseCase, RegisterChatChannelUseCase>();
            services.AddScoped<IGetChatChannelUseCase, GetChatChannelUseCase>();
            services.AddScoped<IUnregisterChatChannelUseCase, UnregisterChatChannelUseCase>();
            services.AddScoped<IQueryServerChatUseCase, QueryServerChatUseCase>();

            return services;
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            services.AddScoped<RegisterChatChannelRunner>();
            services.AddScoped<UnregisterChatChannelRunner>();
            services.AddScoped<QueryServerChatRunner>();

            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, RegisterChatChannelCommand>();
            services.AddSingleton<IOttdSlashCommand, UnregisterChatChannelCommand>();
            services.AddSingleton<IOttdSlashCommand, QueryServerChatCommand>();

            return services;
        }
    }
}
