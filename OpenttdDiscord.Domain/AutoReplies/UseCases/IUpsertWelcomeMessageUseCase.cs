namespace OpenttdDiscord.Domain.AutoReplies.UseCases
{
    public interface IUpsertWelcomeMessageUseCase
    {
        Task Execute(
            ulong guildId,
            Guid serverId,
            string content);
    }
}