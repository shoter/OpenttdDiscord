using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Statuses.UseCases
{
    public interface ICheckIfStatusMonitorExistsUseCase
    {
        EitherAsync<IError, bool> Execute(User user, Guid serverId, ulong channelId);
    }
}
