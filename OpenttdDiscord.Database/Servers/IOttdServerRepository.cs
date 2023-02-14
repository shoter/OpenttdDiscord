using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Database.Servers
{
    public interface IOttdServerRepository
    {
        Task<Either<IError, Result<Unit>>> InsertServer(OttdServer server);

        Task<Either<IError, Result<Unit>>> UpdateServer(OttdServer server);

        Task<Either<IError, Result<Unit>>> DeleteServer(Guid serverId);

        Task<Result<IReadOnlyList<OttdServer>>> GetServersForGuild(long guildId);

        Task<Either<IError, Result<OttdServer>>> GetServer(Guid serverId);

    }
}
