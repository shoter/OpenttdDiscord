using LanguageExt;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReplies.UseCases
{
    public class UpsertWelcomeMessageUseCase : IUpsertWelcomeMessageUseCase
    {
        private readonly IAutoReplyRepository autoReplyRepository;
        private readonly IAkkaService akkaService;

        public UpsertWelcomeMessageUseCase(
            IAutoReplyRepository autoReplyRepository,
            IAkkaService akkaService)
        {
            this.autoReplyRepository = autoReplyRepository;
            this.akkaService = akkaService;
        }

        public EitherAsyncUnit Execute(
            ulong guildId,
            Guid serverId,
            string content)
        {
            WelcomeMessage newWelcomMessage = new(
                serverId,
                content);

            UpdateWelcomeMessage msg = new(
                guildId,
                serverId,
                content);

            return from _1 in autoReplyRepository.UpsertWelcomeMessage(
                    guildId,
                    newWelcomMessage)
                from selection in akkaService.SelectActor(MainActors.Paths.Guilds)
                from _2 in selection.TryAsk(msg)
                select Unit.Default;
        }
    }
}