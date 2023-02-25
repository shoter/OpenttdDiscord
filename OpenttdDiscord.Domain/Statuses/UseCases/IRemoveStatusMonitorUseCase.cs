using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Statuses.UseCases
{
    public interface IRemoveStatusMonitorUseCase
    {
        EitherAsyncUnit Execute(User user, Guid serverId, ulong channelId);
    }
}
