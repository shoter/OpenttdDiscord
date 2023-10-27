namespace OpenttdDiscord.Domain.AutoReplies
{
    public interface IAutoReplyRepository
    {
        EitherAsyncUnit InsertWelcomeMessage(
            ulong guildId,
            WelcomeMessage welcomeMessage);

        EitherAsync<IError, Option<WelcomeMessage>> GetWelcomeMessage(
            ulong guildId,
            Guid serverId)

        EitherAsyncUnit UpdateWelcomeMessage(
            ulong guildId,
            WelcomeMessage welcomeMessage);
    }
}