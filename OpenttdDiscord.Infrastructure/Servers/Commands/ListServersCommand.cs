using Discord;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Servers.Runners;

namespace OpenttdDiscord.Infrastructure.Servers.Commands
{
    internal class ListServersCommand : OttdSlashCommandBase<ListServerRunner>
    {
        public ListServersCommand() : base("list-ottd-servers")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder.WithDescription("List all registered ottd servers on this discord server");
        }
    }
}
