using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Roles;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Roles.Commands;
using OpenttdDiscord.Infrastructure.Roles.Runners;
using OpenttdDiscord.Infrastructure.Roles.UseCases;

namespace OpenttdDiscord.Infrastructure.Roles
{
    public class RolesModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services
                .AddScoped<IRolesRepository, RolesRepository>()
                .RegisterUseCases()
                .RegisterCommands()
                .RegisterRunners();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal static class RolesModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterUseCases(this IServiceCollection services)
        {
            return
                services.AddScoped<IGetRoleLevelUseCase, GetRoleLevelUseCase>();
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            services.AddScoped<RegisterBotRoleRunner>();
            services.AddScoped<GetRoleRunner>();
            services.AddScoped<GetGuildRolesRunner>();

            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, RegisterBotRoleCommand>();
            services.AddSingleton<IOttdSlashCommand, GetRoleCommand>();
            services.AddSingleton<IOttdSlashCommand, GetGuildRolesCommand>();

            return services;
        }
    }
}