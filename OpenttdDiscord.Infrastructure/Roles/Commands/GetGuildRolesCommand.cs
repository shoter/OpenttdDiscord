using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Roles.Commands
{
    internal class GetGuildRolesCommand : OttdSlashCommandBase<GetGuildRolesRunner>
    {
        public GetGuildRolesCommand()
            : base("get-roles")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Gets all guild roles which have preconfigured user level within OpenttdDiscord bot.");
        }
    }
}