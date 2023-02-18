using Discord;
using OpenttdDiscord.Infrastructure.Discord;

namespace OpenttdDiscord.Infrastructure.Servers
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
