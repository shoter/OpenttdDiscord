namespace OpenttdDiscord.Domain.AutoReplies.UseCases
{
    public interface IRemoveAutoReplyUseCase
    {
        public EitherAsyncUnit Execute(
            ulong guildId,
            string serverName,
            string triggerMessage);
    }
}