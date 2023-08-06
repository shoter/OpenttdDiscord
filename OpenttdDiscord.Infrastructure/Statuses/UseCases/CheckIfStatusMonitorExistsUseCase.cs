using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal class CheckIfStatusMonitorExistsUseCase : UseCaseBase, ICheckIfStatusMonitorExistsUseCase
    {
        private readonly IStatusMonitorRepository statusMonitorRepository;

        public CheckIfStatusMonitorExistsUseCase(IStatusMonitorRepository statusMonitorRepository)
        {
            this.statusMonitorRepository = statusMonitorRepository;
        }

        public EitherAsync<IError, bool> Execute(User user, Guid serverId, ulong channelId)
        {
            return
            from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
            from monitorOption in statusMonitorRepository.GetStatusMonitor(serverId, channelId)
            select monitorOption.IsSome;
        }
    }
}
