using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal interface IRegisterStatusMonitorUseCase
    {
        Task<EitherUnit> RegisterStatusMonitor(StatusMonitor statusMonitor);
    }
}
