using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.AutoReply.CommandRunners;
using OpenttdDiscord.Infrastructure.AutoReply.Commands;
using OpenttdDiscord.Infrastructure.AutoReply.ModalRunners;
using OpenttdDiscord.Infrastructure.AutoReply.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.AutoReply
{
    public class AutoReplyModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services
                .AddScoped<IAutoReplyRepository, AutoReplyRepository>()
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
            return services
                .AddScoped<IUpsertWelcomeMessageUseCase, UpsertWelcomeMessageUseCase>();
        }

        public static IServiceCollection RegisterRunners(this IServiceCollection services)
        {
            return services.AddScoped<SetWelcomeMessageCommandRunner>()
                .AddScoped<SetWelcomeMessageModalRunner>();
        }

        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            return services
                .AddSingleton<IOttdSlashCommand, SetWelcomeMessageCommand>();
        }
    }
}