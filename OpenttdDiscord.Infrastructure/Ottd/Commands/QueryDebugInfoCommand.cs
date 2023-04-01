using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Ottd.Runners;

namespace OpenttdDiscord.Infrastructure.Ottd.Commands
{
    internal class QueryDebugInfoCommand : OttdSlashCommandBase<QueryDebugInfoRunner>
    {
        public QueryDebugInfoCommand()
            : base("query-debug-info")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Query debug information about server. Used mianly for plugin development")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithDescription("Server name")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
