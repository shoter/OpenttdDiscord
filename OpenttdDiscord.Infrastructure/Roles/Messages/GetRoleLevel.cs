using OpenttdDiscord.Infrastructure.Guilds.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.Messages
{
    public record GetRoleLevel(
        ulong GuildId,
        IEnumerable<ulong> RoleIds) : IGuildMessage
    {
        public GetRoleLevel(
            ulong guildId,
            ulong roleId)
            : this(
                guildId,
                new[] { roleId })
        {
        }
    }
}