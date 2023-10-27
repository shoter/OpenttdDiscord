namespace OpenttdDiscord.Domain.Servers
{
    public interface IOttdServerRepository
    {
        Task<EitherUnit> InsertServer(OttdServer server);

        Task<EitherUnit> UpdateServer(OttdServer server);

        Task<EitherUnit> DeleteServer(Guid serverId);

        EitherAsync<IError, List<OttdServer>> GetServersForGuild(ulong guildId);

        EitherAsync<IError, OttdServer> GetServer(Guid serverId);

        EitherAsync<IError, OttdServer> GetServerByName(ulong guildId, string serverName);

        EitherAsync<IError, List<ulong>> GetAllGuilds();
    }
}
