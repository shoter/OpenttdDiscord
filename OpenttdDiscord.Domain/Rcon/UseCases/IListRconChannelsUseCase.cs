using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Rcon.UseCases
{
    public interface IListRconChannelsUseCase
    {
        EitherAsync<IError, List<RconChannel>> Execute(User user, Guid serverId);

        EitherAsync<IError, List<RconChannel>> Execute(User user, ulong guildId);
    }
}
