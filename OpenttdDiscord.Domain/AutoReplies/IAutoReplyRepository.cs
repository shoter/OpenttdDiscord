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

        EitherAsync<IError, Option<AutoReply>> GetAutoReply(
            ulong guildId,
            Guid serverId,
            string triggerMessage);

        EitherAsyncUnit UpsertAutoReply(
            ulong guildId,
            Guid serverId,
            AutoReply autoReply);

        EitherAsync<IError, IReadOnlyCollection<AutoReply>> GetAutoReplies(
            ulong guildId,
            Guid serverId);

        EitherAsyncUnit RemoveAutoReply(
            ulong guildId,
            Guid serverId,
            string triggerMessage);
    }
}