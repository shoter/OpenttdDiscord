namespace OpenttdDiscord.Domain.AutoReplies.UseCases
{
    public interface IUpsertWelcomeMessageUseCase
    {
        EitherAsyncUnit Execute(
            ulong guildId,
            Guid serverId,
            string content);
    }
}