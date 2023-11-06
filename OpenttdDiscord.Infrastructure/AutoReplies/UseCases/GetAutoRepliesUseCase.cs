using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;

namespace OpenttdDiscord.Infrastructure.AutoReplies.UseCases
{
    public class GetAutoRepliesUseCase : IGetAutoRepliesUseCase
    {
        private readonly IAutoReplyRepository autoReplyRepository;

        public GetAutoRepliesUseCase(IAutoReplyRepository autoReplyRepository)
        {
            this.autoReplyRepository = autoReplyRepository;
        }

        public EitherAsync<IError, IReadOnlyCollection<AutoReply>> Execute(
            ulong guildId,
            Guid serverId) => autoReplyRepository.GetAutoReplies(
            guildId,
            serverId);
    }
}
