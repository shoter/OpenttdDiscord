using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure.Roles.Messages
{
    public record RegisterNewRole(
        ulong GuildId,
        ulong RoleId,
        UserLevel RoleLevel);
}