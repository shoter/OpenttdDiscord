using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Rcon.UseCases
{
    public interface IGetRconChannelUseCase
    {
        EitherAsync<IError, RconChannel> Execute(Guid serverId, ulong guildId, ulong channelId);

        EitherAsync<IError, List<RconChannel>> Execute(Guid serverId, ulong guildId);
    }
}
