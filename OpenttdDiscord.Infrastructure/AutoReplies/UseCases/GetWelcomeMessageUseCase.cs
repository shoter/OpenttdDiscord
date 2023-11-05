using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;

namespace OpenttdDiscord.Infrastructure.AutoReplies.UseCases
{
    public class GetWelcomeMessageUseCase : IGetWelcomeMessageUseCase
    {
        private readonly IAutoReplyRepository autoReplyRepository;

        public GetWelcomeMessageUseCase(IAutoReplyRepository autoReplyRepository)
        {
            this.autoReplyRepository = autoReplyRepository;
        }

        public EitherAsync<IError, Option<WelcomeMessage>> Execute(
            ulong guildId,
            Guid serverId) => autoReplyRepository.GetWelcomeMessage(
            guildId,
            serverId);
    }
}
