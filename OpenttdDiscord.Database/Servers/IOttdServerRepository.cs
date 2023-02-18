using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Database.Servers
{
    public interface IOttdServerRepository
    {
        Task<EitherUnit> InsertServer(OttdServer server);
        Task<EitherUnit> UpdateServer(OttdServer server);
        Task<EitherUnit> DeleteServer(Guid serverId);
        Task<Either<IError, List<OttdServer>>> GetServersForGuild(ulong guildId);
        Task<Either<IError, OttdServer>> GetServer(Guid serverId);
        Task<Either<IError, OttdServer>> GetServerByName(string serverName);
        Task<Either<IError, List<ulong>>> GetAllGuilds();
    }
}
