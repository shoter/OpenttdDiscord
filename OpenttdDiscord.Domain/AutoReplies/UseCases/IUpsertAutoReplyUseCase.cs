namespace OpenttdDiscord.Domain.AutoReplies.UseCases
{
    public interface IUpsertAutoReplyUseCase
    {
        EitherAsyncUnit Execute(
            ulong guildId,
            Guid serverId,
            AutoReply autoReply);
    }
}