using LanguageExt;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Roles;

namespace OpenttdDiscord.Database.Roles
{
    internal class RolesRepository : IRolesRepository
    {
        public RolesRepository(OttdContext db)
        {
            Db = db;
        }

        private OttdContext Db { get; }

        public EitherAsync<IError, Unit> InsertRole(GuildRole role) => TryAsync<EitherUnit>(
                async () =>
                {
                    await Db
                        .GuildRoles
                        .AddAsync(
                            new GuildRoleEntity(
                                role.GuildId,
                                role.RoleId,
                                (int)role.RoleLevel));

                    await Db.SaveChangesAsync();

                    return Unit.Default;
                })
            .ToEitherAsyncErrorFlat();

        public EitherAsync<IError, List<GuildRole>> GetRoles(ulong guildId)
        => TryAsync<Either<IError, List<GuildRole>>>(
        async () =>
        {
            return (await Db
                    .GuildRoles
                    .Where(r => r.GuildId == guildId)
                    .ToListAsync())
                .Select(x => x.ToDomain())
                .ToList();
        }).ToEitherAsyncErrorFlat();

        public EitherAsync<IError, Unit> DeleteRole(
            ulong guildId,
            ulong roleId) => TryAsync<EitherUnit>(
                async () =>
                {
                    await Db
                        .GuildRoles
                        .Where(x => x.GuildId == guildId && x.RoleId == roleId)
                        .DeleteFromQueryAsync();

                    return Unit.Default;
                })
            .ToEitherAsyncErrorFlat();
    }
}