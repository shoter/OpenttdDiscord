using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Roles
{
    public interface IRolesRepository
    {
        EitherAsyncUnit InsertRole(GuildRole role);
        EitherAsync<IError, List<GuildRole>> GetRoles(ulong guildId);
        EitherAsyncUnit DeleteRole(
            ulong guildId,
            ulong roleId);

        EitherAsyncUnit DeleteRole(GuildRole role) => DeleteRole(
            role.GuildId,
            role.RoleId);
    }
}