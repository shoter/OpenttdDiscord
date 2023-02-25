using Discord;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Discord.Commands
{
    public interface IOttdSlashCommand
    {
        string Name { get; }

        SlashCommandProperties Build();

        IOttdSlashCommandRunner CreateRunner(IServiceProvider sp);
    }
}
