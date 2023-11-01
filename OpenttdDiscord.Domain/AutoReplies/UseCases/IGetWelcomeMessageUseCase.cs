namespace OpenttdDiscord.Domain.AutoReplies.UseCases
{
    public interface IGetWelcomeMessageUseCase
    {
        EitherAsync<IError, Option<WelcomeMessage>> Execute(
            ulong guildId,
            Guid serverId);
    }
}