using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Guilds.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.Messages
{
    public record RegisterNewRole(
        ulong GuildId,
        ulong RoleId,
        UserLevel RoleLevel) : IGuildMessage;
}