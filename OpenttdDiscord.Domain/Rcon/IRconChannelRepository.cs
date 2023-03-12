using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Rcon
{
    public interface IRconChannelRepository
    {
        EitherAsyncUnit Insert(RconChannel rconChannel);

        EitherAsyncUnit Delete(Guid serverId, ulong channelId);

        EitherAsync<IError, List<RconChannel>> GetRconChannelsForTheServer(Guid serverId);

        EitherAsync<IError, List<RconChannel>> GetRconChannelsForTheGuild(ulong guildId);

        EitherAsync<IError, Option<RconChannel>> GetRconChannel(Guid serverId, ulong channelId);
    }
}
