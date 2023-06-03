using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database;
using OpenttdDiscord.Domain.Roles;

namespace OpenttdDiscord.Infrastructure.Roles
{
    public class RolesRepository : IRolesRepository
    {
        public RolesRepository(OttdContext db)
        {
            Db = db;
        }

        private OttdContext Db { get; }

        public EitherAsync<IError, Unit> InsertRole(GuildRole role)
        {
            throw new NotImplementedException();
        }

        public EitherAsync<IError, List<GuildRole>> GetRoles(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public EitherAsync<IError, Unit> DeleteRole(
            ulong guildId,
            ulong roleId)
        {
            throw new NotImplementedException();
        }
    }
}