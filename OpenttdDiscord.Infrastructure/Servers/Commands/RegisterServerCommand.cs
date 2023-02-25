using Discord;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Servers.Runners;

namespace OpenttdDiscord.Infrastructure.Servers.Commands
{
    internal class RegisterServerCommand : OttdSlashCommandBase<RegisterServerRunner>
    {
        public RegisterServerCommand() : base("register-ottd-server")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder.WithDescription("Register information about new ottd server");

            builder.AddOption(new SlashCommandOptionBuilder()
                .WithRequired(true)
                .WithName("name")
                .WithDescription("Name of the server - write anything you like.")
                .WithType(ApplicationCommandOptionType.String));

            builder.AddOption(new SlashCommandOptionBuilder()
                .WithRequired(true)
                .WithName("password")
                .WithDescription("Admin port password.")
                .WithType(ApplicationCommandOptionType.String));

            builder.AddOption(new SlashCommandOptionBuilder()
                .WithRequired(true)
                .WithName("ip")
                .WithDescription("Ip address of the server")
                .WithType(ApplicationCommandOptionType.String));

            builder.AddOption(new SlashCommandOptionBuilder()
                .WithRequired(true)
                .WithName("port")
                .WithDescription("AdminPort port")
                .WithType(ApplicationCommandOptionType.Integer));
        }
    }
}
