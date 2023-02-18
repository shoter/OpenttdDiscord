using Discord;
using OpenttdDiscord.Infrastructure.Discord;

namespace OpenttdDiscord.Infrastructure.Servers
{
    public class ListServersCommand : OttdSlashCommandBase<ListServerRunner>
    {
        public ListServersCommand() : base("register-ottd-server")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder.WithDescription("List all registered ottd servers on this discord server");
        }
    }
}
