using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReplies.UseCases
{
    public class UpsertAutoReplyUseCase : IUpsertAutoReplyUseCase
    {
        private readonly IAutoReplyRepository autoReplyRepository;
        private readonly IAkkaService akkaService;

        public UpsertAutoReplyUseCase(
            IAutoReplyRepository autoReplyRepository,
            IAkkaService akkaService)
        {
            this.autoReplyRepository = autoReplyRepository;
            this.akkaService = akkaService;
        }

        public EitherAsyncUnit Execute(
            ulong guildId,
            Guid serverId,
            AutoReply autoReply)
        {
            return
                from _1 in autoReplyRepository.UpsertAutoReply(
                    guildId,
                    serverId,
                    autoReply)
                from selection in akkaService.SelectActor(MainActors.Paths.Guilds)
                from _2 in selection.TellExt(new UpdateAutoReply(guildId,
                                                 serverId,
                                                 autoReply)).ToAsync()
                select Unit.Default;
        }
    }
}