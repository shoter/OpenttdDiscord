using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Roles
{
    public record GuildRole(
        ulong GuildId,
        ulong RoleId,
        UserLevel RoleLevel
        );
}