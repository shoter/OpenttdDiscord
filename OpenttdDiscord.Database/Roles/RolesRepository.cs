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

        public EitherAsyncUnit InsertRole(GuildRole role) => TryAsync<EitherUnit>(
                async () =>
                {
                    await Db
                        .GuildRoles
                        .AddAsync(
                            new GuildRoleEntity(
                                role.GuildId,
                                role.RoleId,
                                (int) role.RoleLevel));

                    await Db.SaveChangesAsync();

                    return Unit.Default;
                })
            .ToEitherAsyncErrorFlat();

        public EitherAsyncUnit UpdateRole(GuildRole updatedRole) => TryAsync<EitherUnit>(
                async () =>
                {
                    var role = await Db
                        .GuildRoles
                        .Where(
                            gr =>
                                gr.GuildId == updatedRole.GuildId &&
                                gr.RoleId == updatedRole.RoleId)
                        .FirstOrDefaultAsync();

                    if (role == null)
                    {
                        return new ExceptionError("Role should not be null here!");
                    }

                    role.UserLevel = (int) updatedRole.RoleLevel;
                    await Db.SaveChangesAsync();
                    return Unit.Default;
                })
            .ToEitherAsyncErrorFlat();


        public EitherAsync<IError, List<GuildRole>> GetRoles(ulong guildId) =>
            TryAsync<Either<IError, List<GuildRole>>>(
                    async () =>
                    {
                        return (await Db
                                .GuildRoles
                                .Where(r => r.GuildId == guildId)
                                .ToListAsync())
                            .Select(x => x.ToDomain())
                            .ToList();
                    })
                .ToEitherAsyncErrorFlat();

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