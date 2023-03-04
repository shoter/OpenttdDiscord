using LanguageExt;
using OpenttdDiscord.Domain.DiscordRelated;
using OpenttdDiscord.Domain.Rcon;

namespace OpenttdDiscord.Database.Rcon
{
    internal interface IRconChannelRepository
    {
        EitherAsyncUnit Insert(RconChannel rconChannel);

        EitherAsyncUnit Delete(Guid serverId, ulong channelId);

        EitherAsync<IError, List<RconChannel>> GetRconChannelsForTheServer(Guid serverId);

        EitherAsync<IError, Option<RconChannel>> GetRconChannel(Guid serverId, ulong channelId);
    }
}
