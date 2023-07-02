using OpenttdDiscord.Infrastructure.Guilds.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.Messages
{
    public record DeleteRole(
        ulong GuildId,
        ulong RoleId) : IGuildMessage;
}