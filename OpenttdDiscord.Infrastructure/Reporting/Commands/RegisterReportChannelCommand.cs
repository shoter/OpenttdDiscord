using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Reporting.Runners;

namespace OpenttdDiscord.Infrastructure.Reporting.Commands
{
    internal class RegisterReportChannelCommand : OttdSlashCommandBase<RegisterReportChannelRunner>
    {
        public RegisterReportChannelCommand()
            : base("register-report-channel")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Makes this channel a place for uploading user's reports. Single discord channel can be used by multiple servers")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
