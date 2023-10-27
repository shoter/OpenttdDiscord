using Discord;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.Discord.Commands
{
    public interface IOttdSlashCommand
    {
        string Name { get; }

        SlashCommandProperties Build();

        IOttdSlashCommandRunner CreateRunner(IServiceProvider sp);
    }
}
