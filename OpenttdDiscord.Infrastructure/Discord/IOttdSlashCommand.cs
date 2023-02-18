using Discord;
using Microsoft.Extensions.DependencyInjection;

namespace OpenttdDiscord.Infrastructure.Discord
{
    public interface IOttdSlashCommand
    {
        string Name { get; }

        SlashCommandProperties Build();

        IOttdSlashCommandRunner CreateRunner(IServiceProvider sp);
    }
}
