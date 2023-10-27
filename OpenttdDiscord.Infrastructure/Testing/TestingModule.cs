using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Discord.Modals;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Testing.CommandRunners;
using OpenttdDiscord.Infrastructure.Testing.Commands;
using OpenttdDiscord.Infrastructure.Testing.ModalRunners;
using OpenttdDiscord.Infrastructure.Testing.Modals;

namespace OpenttdDiscord.Infrastructure.Testing
{
    internal class TestingModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services
                .AddScoped<IStatusMonitorRepository, StatusMonitorRepository>()
                .RegisterCommandRunners()
                .RegisterCommands()
                .RegisterModals()
                .RegisterModalRunners();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal static class StatusesModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterCommandRunners(this IServiceCollection services)
        {
            services.AddScoped<TestCommandRunner>();
            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, TestCommand>();
            return services;
        }

        public static IServiceCollection RegisterModals(this IServiceCollection services)
        {
            services.AddSingleton<TestModal>();
            services.AddSingleton<IOttdModal, TestModal>();
            return services;
        }

        public static IServiceCollection RegisterModalRunners(this IServiceCollection services)
        {
            services.AddScoped<TestModalRunner>();
            return services;
        }
    }
}