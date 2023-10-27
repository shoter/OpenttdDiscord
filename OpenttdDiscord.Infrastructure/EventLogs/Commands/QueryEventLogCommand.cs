using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.EventLogs.Runners;

namespace OpenttdDiscord.Infrastructure.EventLogs.Commands
{
    internal class QueryEventLogCommand : OttdSlashCommandBase<QueryEventLogRunner>
    {
        public QueryEventLogCommand()
            : base("query-event-log")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Returns file with latest events from the server")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
