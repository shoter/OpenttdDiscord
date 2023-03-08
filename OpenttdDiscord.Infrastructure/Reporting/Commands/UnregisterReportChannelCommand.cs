using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Reporting.Runners;

namespace OpenttdDiscord.Infrastructure.Reporting.Commands
{
    internal class UnregisterReportChannelCommand : OttdSlashCommandBase<UnregisterReportChannelRunner>
    {
        public UnregisterReportChannelCommand()
         : base("unregister-report-channel")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Unregisters report channel for given discord channel and server")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
