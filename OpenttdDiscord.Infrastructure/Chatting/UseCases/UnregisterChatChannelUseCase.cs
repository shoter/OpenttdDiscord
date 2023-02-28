using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.UseCases
{
    internal class UnregisterChatChannelUseCase : UseCaseBase, IUnregisterChatChannelUseCase
    {
        private readonly IStatusMonitorRepository statusMonitorRepository;

        private readonly IAkkaService akkaService;

        public UnregisterChatChannelUseCase(IStatusMonitorRepository statusMonitorRepository, IAkkaService akkaService)
        {
            this.statusMonitorRepository = statusMonitorRepository;
            this.akkaService = akkaService;
        }

        public EitherAsyncUnit Execute(User user, Guid serverId, ulong guildId, ulong chatChannelId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from _2 in statusMonitorRepository.RemoveStatusMonitor(serverId, chatChannelId)
                from actor in akkaService.SelectActor(MainActors.Paths.Guilds)
                from _3 in actor.TellExt(new UnregisterChatChannel(serverId, guildId, chatChannelId)).ToAsync()
                select _3;
        }
    }
}
