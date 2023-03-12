using LanguageExt;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Statuses.Messages;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal class RemoveStatusMonitorUseCase : UseCaseBase, IRemoveStatusMonitorUseCase
    {
        private readonly IStatusMonitorRepository statusMonitorRepository;
        private readonly IAkkaService akkaService;

        public RemoveStatusMonitorUseCase(
            IStatusMonitorRepository statusMonitorRepository,
            IAkkaService akkaService)
        {
            this.statusMonitorRepository = statusMonitorRepository;
            this.akkaService = akkaService;
        }

        public EitherAsyncUnit Execute(User user, Guid serverId, ulong guildId, ulong channelId)
        {
            var msg = new RemoveStatusMonitor(serverId, guildId, channelId);

            return
            from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
            from _2 in statusMonitorRepository.RemoveStatusMonitor(serverId, channelId)
            from guilds in akkaService.SelectActor(MainActors.Paths.Guilds)
            from _3 in guilds.TryAsk(msg, TimeSpan.FromSeconds(1))
            select Unit.Default;
        }
    }
}
