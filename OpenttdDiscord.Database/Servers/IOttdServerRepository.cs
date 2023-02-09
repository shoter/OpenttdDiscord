using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Database.Servers
{
    public interface IOttdServerRepository
    {
        Task<Result<Unit>> InsertServer(OttdServer server);

        Task<Result<Unit>> UpdateServer(OttdServer server);

        Task<Result<Unit>> DeleteServer(Guid serverId);

        Task<Result<IReadOnlyList<OttdServer>>> GetServersForGuild(long guildId);
    }
}
