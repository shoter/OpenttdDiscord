using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;

namespace OpenttdDiscord.Infrastructure.AutoReplies.UseCases
{
    public class GetAutoReplyUseCase : IGetAutoRepliesUseCase
    {
        private readonly IAutoReplyRepository autoReplyRepository;

        public GetAutoReplyUseCase(IAutoReplyRepository autoReplyRepository)
        {
            this.autoReplyRepository = autoReplyRepository;
        }

        public EitherAsync<IError, IReadOnlyCollection<AutoReply>> Execute(
            ulong guildId,
            Guid serverId) => autoReplyRepository.GetAutoReplies(
            guildId,
            serverId);

        public EitherAsync<IError, Option<AutoReply>> Execute(
            ulong guildId,
            Guid serverId,
            string triggerMessage) => autoReplyRepository.GetAutoReply(
            guildId,
            serverId,
            triggerMessage);
    }
}