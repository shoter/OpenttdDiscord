using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Reporting;
using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Reporting.Commands;
using OpenttdDiscord.Infrastructure.Reporting.Runners;
using OpenttdDiscord.Infrastructure.Reporting.UseCases;

namespace OpenttdDiscord.Infrastructure.Reporting
{
    internal class ReportingModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IReportChannelRepository, ReportChannelRepository>();
            services
                .RegisterUseCases()
                .RegisterCommands()
                .RegisterRunners();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal static class ReportingModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterUseCases(this IServiceCollection services)
        {
            services.AddScoped<IRegisterReportChannelUseCase, RegisterReportChannelUseCase>();
            services.AddScoped<IListReportChannelsUseCase, ListReportChannelsUseCase>();

            return services;
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            services.AddScoped<RegisterReportChannelRunner>();

            return services;
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddSingleton<IOttdSlashCommand, RegisterReportChannelCommand>();

            return services;
        }
    }
}
