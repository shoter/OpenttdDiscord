using OpenttdDiscord.Infrastructure.Guilds.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.Messages
{
    public record GetRoleLevel(
        ulong GuildId,
        ulong RoleId) : IGuildMessage;
}