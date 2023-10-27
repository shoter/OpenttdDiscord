using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Rcon.UseCases
{
    public interface IGetRconChannelUseCase
    {
        EitherAsync<IError, Option<RconChannel>> Execute(User user, Guid serverId, ulong channelId);
    }
}
