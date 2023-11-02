namespace OpenttdDiscord.Domain.AutoReplies
{
    public record WelcomeMessage(
        Guid ServerId,
        string Content);
}