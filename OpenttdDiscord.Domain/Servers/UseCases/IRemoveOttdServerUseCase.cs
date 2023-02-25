using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Servers.UseCases
{
    public interface IRemoveOttdServerUseCase
    {
        Task<EitherUnit> Execute(User userRights, ulong guildId, string serverName);
    }
}
