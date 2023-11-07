using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.AutoReplies.Commands;
using OpenttdDiscord.Infrastructure.AutoReplies.ModalRunners;
using OpenttdDiscord.Infrastructure.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.AutoReplies
{
    [ExcludeFromCodeCoverage]
    public class AutoReplyModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services
                .AddScoped<IAutoReplyRepository, AutoReplyRepository>()
                .AddSingleton<IAssociatedModalRunners, AutoReplyAssociatedModalRunners>()
                .RegisterUseCases()
                .RegisterCommands()
                .RegisterRunners();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    [ExcludeFromCodeCoverage]
    internal static class AutoReplyModuleExtensions
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static IServiceCollection RegisterUseCases(this IServiceCollection services)
        {
            return services
                .AddScoped<IUpsertWelcomeMessageUseCase, UpsertWelcomeMessageUseCase>()
                .AddScoped<IGetWelcomeMessageUseCase, GetWelcomeMessageUseCase>()
                .AddScoped<IGetAutoReplyUseCase, GetAutoReplyUseCase>();
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            return services.AddScoped<SetWelcomeMessageCommandRunner>()
                .AddScoped<SetWelcomeMessageModalRunner>();
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            return services
                .AddSingleton<IOttdSlashCommand, SetWelcomeMessageCommand>()
                .AddSingleton<IOttdSlashCommand, SetAutoReplyCommand>();
        }
    }
}