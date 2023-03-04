using Discord;
using OpenttdDiscord.Infrastructure.Admin.Runners;
using OpenttdDiscord.Infrastructure.Discord.Commands;

namespace OpenttdDiscord.Infrastructure.Admin.Commands
{
    internal class RegisterAdminChannelCommand : OttdSlashCommandBase<RegisterAdminChannelRunner>
    {
        public RegisterAdminChannelCommand()
            : base("register-admin-channel")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Registers channel to interact with server through RCON interface. " +
                "You will be able to execute ottd commands on this channel afterwards. " +
                "DO NOT execute this command on a channel where normal users have access to")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server")
                    .WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("prefix")
                    .WithRequired(true)
                    .WithDescription("Prefix that you need to use in order to execute rcon commands")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
