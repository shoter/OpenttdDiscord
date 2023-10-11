using Discord;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Roles.Commands
{
    internal class RegisterBotRoleCommand : OttdSlashCommandBase<RegisterRoleRunner>
    {
        public RegisterBotRoleCommand()
            : base("register-role")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Registers given role to execute bot commands")
                .AddOption(
                    new SlashCommandOptionBuilder()
                        .WithName("role")
                        .WithDescription("Role name")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Role))
                .AddOption(
                    new SlashCommandOptionBuilder()
                        .WithName("role-level")
                        .WithDescription("Level of access for given role")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer)
                        .AddChoice(
                            "Moderator",
                            (int)UserLevel.Moderator)
                        .AddChoice(
                            "Admin",
                            (int)UserLevel.Admin));
        }
    }
}