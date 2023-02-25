using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Domain.Servers;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace OpenttdDiscord.Database.Servers
{
    internal class OttdServerRepository : IOttdServerRepository
    {
        private OttdContext Db { get; }

        public OttdServerRepository(OttdContext dbContext)
        {
            this.Db = dbContext;
        }

        public async Task<EitherUnit> DeleteServer(Guid serverId)
        {
            return (await new TryAsync<Unit>(async () =>
            {
                await Db.Servers.Where(s => s.Id == serverId).DeleteFromQueryAsync();
                await Db.SaveChangesAsync();
                return Unit.Default;
            }))
           .Match(
               entity => EitherUnit.Right(Unit.Default),
               ex => Left<IError>(new ExceptionError(ex))
           );
        }

        public async Task<EitherUnit> InsertServer(OttdServer server)
        {
            return (await TryAsync(async () =>
            {
                await Db.Servers.AddAsync(new(server));
                await Db.SaveChangesAsync();
                return Unit.Default;
            }))
            .Match(
               entity => EitherUnit.Right(Unit.Default),
               ex => EitherUnit.Left(new ExceptionError(ex))
           );
        }

        public async Task<EitherUnit> UpdateServer(OttdServer server)
        {
            return (await TryAsync<EitherUnit>(async () =>
            {
                var foundServer = await Db.Servers.FindAsync(server.Id);
                if (foundServer == null)
                {
                    return Left<IError>(new HumanReadableError("Server not found"));
                }

                Db.Entry(foundServer).CurrentValues.SetValues(new OttdServerEntity(server));
                await Db.SaveChangesAsync();
                return Right(Unit.Default);
            })).IfFail(ex => new ExceptionError(ex));
        }

        public async Task<Either<IError, List<OttdServer>>> GetServersForGuild(ulong guildId)
        {
            return (await TryAsync<Either<IError, List<OttdServer>>>(async () =>
            {
                List<OttdServerEntity> serverEntities = await Db
                .Servers
                .Where(s => s.GuildId == guildId)
                .ToListAsync();

                return serverEntities
                .Select(s => s.ToDomain())
                .ToList();
            })).IfFail(ex => new ExceptionError(ex));
        }

        public async Task<Either<IError, OttdServer>> GetServer(Guid serverId)
        {
            return (await TryAsync(async () => await Db.FindAsync<OttdServerEntity>(serverId)))
            .Match(
                entity => entity == null
                    ? Either<IError, OttdServer>.Left(new HumanReadableError("Server was not found"))
                    : Either<IError, OttdServer>.Right(entity.ToDomain()),
                ex => Either<IError, OttdServer>.Left(new ExceptionError(ex))
            );
        }

        public EitherAsync<IError, OttdServer> GetServerByName(ulong guildId, string serverName)
            => TryAsync<Either<IError, OttdServer>>(async () =>
        {
            var server = await Db.Servers.SingleOrDefaultAsync(s => s.GuildId == guildId && s.Name == serverName);

            if (server == null)
            {
                return new HumanReadableError("Server not found");
            }
            return server.ToDomain();
        }).ToEitherAsyncErrorFlat();

        public async Task<Either<IError, List<ulong>>> GetAllGuilds()
        {
            try
            {
                return await Db
                    .Servers
                    .Select(x => x.GuildId)
                    .Distinct()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new ExceptionError(ex);
            }

        }
    }
}
