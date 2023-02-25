using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Security;
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
            IAkkaService akkaService )
        {
            this.statusMonitorRepository = statusMonitorRepository;
            this.akkaService = akkaService;
        }

        public EitherAsyncUnit Execute(User user, Guid serverId, ulong guildId, ulong channelId)
        {
            return
            from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
            from _2 in statusMonitorRepository.RemoveStatusMonitor(serverId, channelId)
            from _3 in InformActor(serverId, guildId, channelId)
            select _3;
        }

        private EitherAsyncUnit InformActor(Guid serverId, ulong guildId, ulong channelId)
            => TryAsync(async () =>
            {
                var msg = new RemoveStatusMonitor(serverId, guildId, channelId);
                var actors = await akkaService.SelectActor(MainActors.Paths.Guilds);
                return (await actors.TryAsk(msg))
                    .Map(_ => Unit.Default);
            }).ToEitherAsyncErrorFlat();
    }
}
