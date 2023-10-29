namespace OpenttdDiscord.Domain.AutoReplies
{
    public interface IAutoReplyRepository
    {
        EitherAsyncUnit UpsertWelcomeMessage(
            ulong guildId,
            WelcomeMessage welcomeMessage);

        EitherAsync<IError, Option<WelcomeMessage>> GetWelcomeMessage(
            ulong guildId,
            Guid serverId);
    }
}