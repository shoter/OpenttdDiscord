﻿using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Servers.Commands;
using OpenttdDiscord.Infrastructure.Servers.Runners;
using OpenttdDiscord.Infrastructure.Servers.UseCases;

namespace OpenttdDiscord.Infrastructure.Servers
{
    internal class ServersModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IOttdServerRepository, OttdServerRepository>();
            services
                .RegisterUseCases()
                .RegisterCommands()
                .RegisterRunners();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal static class ServersModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterUseCases(this IServiceCollection services)
        {
            services.AddScoped<IRegisterOttdServerUseCase, RegisterOttdServerUseCase>();
            services.AddScoped<IRemoveOttdServerUseCase, RemoveOttdServerUseCase>();
            services.AddScoped<IGetServerUseCase, GetServerUseCase>();

            return services;
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            services.AddScoped<RegisterServerRunner>();
            services.AddScoped<ListServerRunner>();
            services.AddScoped<RemoveOttdServerRunner>();

            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, RegisterServerCommand>();
            services.AddSingleton<IOttdSlashCommand, ListServersCommand>();
            services.AddSingleton<IOttdSlashCommand, RemoveOttdServerCommand>();

            return services;
        }
    }
}
