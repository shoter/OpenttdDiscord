namespace OpenttdDiscord.Domain.AutoReplies.UseCases
{
    public interface IGetAutoReplyUseCase
    {
        EitherAsync<IError, IReadOnlyCollection<AutoReply>> Execute(
            ulong guildId,
            Guid serverId);

        EitherAsync<IError, Option<AutoReply>> Execute(
            ulong guildId,
            Guid serverId,
            string triggerMessage);
    }
}