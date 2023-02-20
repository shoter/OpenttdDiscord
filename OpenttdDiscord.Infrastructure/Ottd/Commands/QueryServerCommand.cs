using Discord;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Ottd.Runners;

namespace OpenttdDiscord.Infrastructure.Ottd.Commands
{
    internal class QueryServerCommand : OttdSlashCommandBase<QueryServerRunner>
    {
        public QueryServerCommand() : base("query-server")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Displays basic information about the server")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
