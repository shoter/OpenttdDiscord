using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Servers.UseCases
{
    public interface IGetServerUseCase
    {
        EitherAsync<IError, OttdServer> Execute(User user, string serverName, ulong guildId);

        EitherAsync<IError, OttdServer> Execute(User user, Guid serverId);
    }
}
