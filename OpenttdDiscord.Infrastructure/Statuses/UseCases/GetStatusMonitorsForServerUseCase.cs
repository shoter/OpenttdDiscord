using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Domain.Statuses.UseCases;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal class GetStatusMonitorsForServerUseCase : UseCaseBase, IGetStatusMonitorsForServerUseCase
    {
        private readonly IStatusMonitorRepository statusMonitorRepository;

        public GetStatusMonitorsForServerUseCase(IStatusMonitorRepository statusMonitorRepository)
        {
            this.statusMonitorRepository = statusMonitorRepository;
        }

        public EitherAsync<IError, List<StatusMonitor>> Execute(User user, Guid serverId)
        {
            return
            from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
            from servers in statusMonitorRepository.GetStatusMonitors(serverId)
            select servers;
        }
    }
}
