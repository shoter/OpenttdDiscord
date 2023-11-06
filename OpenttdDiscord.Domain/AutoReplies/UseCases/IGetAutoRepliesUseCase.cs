namespace OpenttdDiscord.Domain.AutoReplies.UseCases
{
    public interface IGetAutoRepliesUseCase
    {
        EitherAsync<IError, IReadOnlyCollection<AutoReply>> Execute(
            ulong guildId,
            Guid serverId);
    }
}