using LanguageExt;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;

namespace OpenttdDiscord.Infrastructure.AutoReply.UseCases
{
    public class UpsertWelcomeMessageUseCase : IUpsertWelcomeMessageUseCase
    {
        private readonly IAutoReplyRepository autoReplyRepository;

        public UpsertWelcomeMessageUseCase(IAutoReplyRepository autoReplyRepository)
        {
            this.autoReplyRepository = autoReplyRepository;
        }

        public Task Execute(
            ulong guildId,
            Guid serverId,
            string content)
        {
            WelcomeMessage newWelcomMessage = new(
                serverId,
                content);
            from server in autoReplyRepository.GetWelcomeMessage(guildId, serverId)
                
        }

        private EitherAsyncUnit xxx(
            Option<WelcomeMessage> welcomeMessageOption,
            ulong guildId,
            WelcomeMessage newMessage) => welcomeMessageOption.BiMap(
            some => autoReplyRepository.UpdateWelcomeMessage(
                guildId,
                newMessage),
            none => autoReplyRepository.UpsertWelcomeMessage(
                guildId,
                newMessage));

    }
}
