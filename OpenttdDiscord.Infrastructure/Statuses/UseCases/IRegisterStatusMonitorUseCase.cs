using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal interface IRegisterStatusMonitorUseCase
    {
        EitherAsyncUnit RegisterStatusMonitor(StatusMonitor statusMonitor);
    }
}
