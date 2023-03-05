using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Rcon.Runners;

namespace OpenttdDiscord.Infrastructure.Rcon.Commands
{
    internal class ListRconChannelsCommand : OttdSlashCommandBase<ListRconChannelsRunner>
    {
        public ListRconChannelsCommand()
            : base("list-rcon-channels")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Lists all rcon channels in this guild along with prefixes");
        }
    }
}
