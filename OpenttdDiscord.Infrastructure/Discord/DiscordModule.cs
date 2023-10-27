using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.Discord
{
    internal class DiscordModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services
                .AddSingleton<IDiscordInteractionService, DiscordInteractionService>();
        }
    }
}