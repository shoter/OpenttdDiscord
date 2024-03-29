﻿using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Rcon;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Rcon.Commands;
using OpenttdDiscord.Infrastructure.Rcon.Runners;
using OpenttdDiscord.Infrastructure.Rcon.UseCases;

namespace OpenttdDiscord.Infrastructure.Rcon
{
    internal class RconModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IRconChannelRepository, RconChannelRepository>();
            services
                .RegisterUseCases()
                .RegisterCommands()
                .RegisterRunners();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal static class RconModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterUseCases(this IServiceCollection services)
        {
            services.AddScoped<IRegisterRconChannelUseCase, RegisterRconChannelUseCase>();
            services.AddScoped<IGetRconChannelUseCase, GetRconChannelUseCase>();
            services.AddScoped<IUnregisterRconChannelUseCase, UnregisterRconChannelUseCase>();
            services.AddScoped<IListRconChannelsUseCase, ListRconChannelsUseCase>();

            return services;
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            services.AddScoped<RegisterRconChannelRunner>();
            services.AddScoped<UnregisterRconChannelRunner>();
            services.AddScoped<ListRconChannelsRunner>();

            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, RegisterRconChannelCommand>();
            services.AddSingleton<IOttdSlashCommand, UnregisterRconChannelCommand>();
            services.AddSingleton<IOttdSlashCommand, ListRconChannelsCommand>();

            return services;
        }
    }
}
