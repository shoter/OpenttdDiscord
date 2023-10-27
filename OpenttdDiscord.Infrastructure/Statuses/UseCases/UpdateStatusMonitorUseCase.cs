using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Domain.Statuses.UseCases;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal class UpdateStatusMonitorUseCase : UseCaseBase, IUpdateStatusMonitorUseCase
    {
        private readonly IStatusMonitorRepository statusMonitorRepository;

        public UpdateStatusMonitorUseCase(IStatusMonitorRepository statusMonitorRepository)
        {
            this.statusMonitorRepository = statusMonitorRepository;
        }

        public EitherAsync<IError, StatusMonitor> Execute(
            User user,
            StatusMonitor monitor)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from statusMonitor in statusMonitorRepository.UpdateStatusMonitor(monitor)
                select statusMonitor;
        }
    }
}