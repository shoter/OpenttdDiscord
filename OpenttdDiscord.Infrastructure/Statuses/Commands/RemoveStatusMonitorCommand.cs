using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Statuses.Runners;

namespace OpenttdDiscord.Infrastructure.Statuses.Commands
{
    internal class RemoveStatusMonitorCommand : OttdSlashCommandBase<RemoveStatusMonitorRunner>
    {
        public RemoveStatusMonitorCommand()
            : base("remove-status-monitor")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Removes status monitor")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
