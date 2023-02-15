using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Database.Servers
{
    public interface IOttdServerRepository
    {
        Task<Either<IError, Unit>> InsertServer(OttdServer server);

        Task<Either<IError, Unit>> UpdateServer(OttdServer server);

        Task<Either<IError, Unit>> DeleteServer(Guid serverId);

        Task<Result<IReadOnlyList<OttdServer>>> GetServersForGuild(long guildId);

        Task<Either<IError, OttdServer>> GetServer(Guid serverId);

    }
}
