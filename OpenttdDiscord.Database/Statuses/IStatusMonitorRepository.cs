using LanguageExt;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Database.Statuses
{
    internal interface IStatusMonitorRepository
    {
        Task<EitherUnit> Insert(StatusMonitor entity);

        Task<Either<IError, List<StatusMonitor>>> GetStatusMonitors(Guid severId);

        Task<EitherUnit> RemoveStatusMonitor(Guid serverId, ulong channelId);

        Task<Either<IError, StatusMonitor>> UpdateStatusMonitor(StatusMonitor entity);
    }
}
