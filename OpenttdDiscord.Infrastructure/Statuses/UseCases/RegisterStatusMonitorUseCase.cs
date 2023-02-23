using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal class RegisterStatusMonitorUseCase : IRegisterStatusMonitorUseCase
    {
        private readonly IStatusMonitorRepository statusMonitorRepository;

        public RegisterStatusMonitorUseCase(IStatusMonitorRepository statusMonitorRepository)
        {
            this.statusMonitorRepository = statusMonitorRepository;
        }

        public Task<EitherUnit> RegisterStatusMonitor(StatusMonitor statusMonitor)
        {
            return statusMonitorRepository.Insert(statusMonitor);
        }
    }
}
