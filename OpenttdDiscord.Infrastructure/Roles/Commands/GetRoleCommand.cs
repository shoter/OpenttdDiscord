using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Roles.Commands
{
    internal class GetRoleCommand : OttdSlashCommandBase<GetRoleRunner>
    {
        public GetRoleCommand()
            : base("get-my-role")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Gets your current role on the server");
        }
    }
}