using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Rcon.Runners;

namespace OpenttdDiscord.Infrastructure.Rcon.Commands
{
    internal class UnregisterRconChannelCommand : OttdSlashCommandBase<UnregisterRconChannelRunner>
    {
        public UnregisterRconChannelCommand()
            : base("unregister-rcon-channel")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Unregisters rcon channel for given discord channel and server")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
