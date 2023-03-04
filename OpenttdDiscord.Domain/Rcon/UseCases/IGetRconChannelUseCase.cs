using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Rcon.UseCases
{
    public interface IGetRconChannelUseCase
    {
        EitherAsync<IError, RconChannel> Execute(User user, Guid serverId, ulong guildId, ulong channelId);

        EitherAsync<IError, List<RconChannel>> Execute(User user, Guid serverId, ulong guildId);
    }
}
