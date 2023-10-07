using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Roles.Commands
{
    internal class DeleteRoleCommand : OttdSlashCommandBase<DeleteRoleRunner>
    {
        public DeleteRoleCommand()
            : base("delete-role")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Deletes permissions for given role")
                .AddOption(
                    new SlashCommandOptionBuilder()
                        .WithName("role")
                        .WithDescription("Role name")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Role));
        }
    }
}