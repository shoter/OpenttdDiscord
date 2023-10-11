using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Reporting.Runners;

namespace OpenttdDiscord.Infrastructure.Reporting.Commands
{
    internal class ListReportChannelsCommand : OttdSlashCommandBase<ListReportChannelsRunner>
    {
        public ListReportChannelsCommand()
            : base("list-report-channels")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Lists all report channels for guild you are in");
        }
    }
}
