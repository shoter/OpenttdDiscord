using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Maintenance.Runners;

namespace OpenttdDiscord.Infrastructure.Maintenance.Commands
{
    internal class HealthCheckCommand : OttdSlashCommandBase<HealthCheckRunner>
    {
        public HealthCheckCommand()
            : base("health-check")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Checks health of application");
        }
    }
}