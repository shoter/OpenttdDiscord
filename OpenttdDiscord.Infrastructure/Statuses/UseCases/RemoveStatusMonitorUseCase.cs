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

        public RemoveStatusMonitorUseCase(
            IStatusMonitorRepository statusMonitorRepository,
            IAkkaService akkaService)
            : base(akkaService)
        {
            this.statusMonitorRepository = statusMonitorRepository;
        }

        public EitherAsyncUnit Execute(
            User user,
            Guid serverId,
            ulong guildId,
            ulong channelId)
        {
            var msg = new RemoveStatusMonitor(
                serverId,
                guildId,
                channelId);

            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from _2 in statusMonitorRepository.RemoveStatusMonitor(
                    serverId,
                    channelId)
                from guilds in AkkaService.SelectActor(MainActors.Paths.Guilds)
                from _3 in guilds.TryAsk(
                    msg,
                    TimeSpan.FromSeconds(1))
                select Unit.Default;
        }
    }
}