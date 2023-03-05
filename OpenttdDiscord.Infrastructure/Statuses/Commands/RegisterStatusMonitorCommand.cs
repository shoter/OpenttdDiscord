using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Statuses.Runners;

namespace OpenttdDiscord.Infrastructure.Statuses.Commands
{
    internal class RegisterStatusMonitorCommand : OttdSlashCommandBase<RegisterStatusMonitorRunner>
    {
        public RegisterStatusMonitorCommand()
            : base("register-status-monitor")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Creates a message that will be regularly updates with server status")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Server name")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
