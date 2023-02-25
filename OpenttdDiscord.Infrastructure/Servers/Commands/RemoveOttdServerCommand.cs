using Discord;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Servers.Runners;

namespace OpenttdDiscord.Infrastructure.Servers.Commands
{
    internal class RemoveOttdServerCommand : OttdSlashCommandBase<RemoveOttdServerRunner>
    {
        public RemoveOttdServerCommand() : base("remove-ottd-server")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Removes given ottd server and all stored settings along with it")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server")
                    .WithType(ApplicationCommandOptionType.String));

        }
    }
}
