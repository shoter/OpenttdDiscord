using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Domain.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<Either<IError, Result<Unit>>> DeleteServer(Guid serverId)
        {
            return await new TryAsync<Unit>(async () =>
            {
                var entity = new OttdServerEntity(
                    serverId,
                    default!,
                    default!,
                    default!,
                    default!,
                    default!,
                    default!
                    );

                var entry = Db.Entry(entity);

                entry.State = EntityState.Deleted;
                await Db.SaveChangesAsync();
                return Unit.Default;
            })();
        }

        public async Task<Either<IError, Result<Unit>>> InsertServer(OttdServer server)
        {
            return (await TryAsync(async () =>
            {
                await Db.Servers.AddAsync(new(server));
                await Db.SaveChangesAsync();
                return Unit.Default;
            }))
           .Match(
               entity => Right(new Result<Unit>(Unit.Default)),
               ex => Left<IError>(new ExceptionError(ex))
           );

        }

        public async Task<Either<IError, Result<Unit>>> UpdateServer(OttdServer server)
        {
            return (await TryAsync<Either<IError, Result<Unit>>>(async () =>
            {
                var foundServer = await Db.Servers.FindAsync(server.Id);
                if (foundServer == null)
                {
                    return Left<IError>(new HumanReadableError("Server not found"));
                }

                Db.Entry(foundServer).CurrentValues.SetValues(new OttdServerEntity(server));
                await Db.SaveChangesAsync();
                return Right(new Result<Unit>(Unit.Default));
            }))
            .Match(
                task => task,
                ex => Left<IError>(new ExceptionError(ex))
            );
        }

        public async Task<Result<IReadOnlyList<OttdServer>>> GetServersForGuild(long guildId)
        {
            return await new TryAsync<IReadOnlyList<OttdServer>>(async () =>
            {
                List<OttdServerEntity> serverEntities = await Db
                .Servers
                .Where(s => s.GuildId == guildId)
                .ToListAsync();

                return serverEntities
                .Select(s => s.ToOttdServer())
                .ToList();
            })();
        }

        public async Task<Either<IError, Result<OttdServer>>> GetServer(Guid serverId)
        {
            return (await TryAsync(async () => await Db.FindAsync<OttdServerEntity>(serverId)))
            .Match(
                entity => entity == null
                    ? Either<IError, Result<OttdServer>>.Left(new HumanReadableError("Server was not found"))
                    : Either<IError, Result<OttdServer>>.Right(entity.ToOttdServer()),
                ex => Either<IError, Result<OttdServer>>.Left(new ExceptionError(ex))
            );
        }
    }
}
