namespace OpenttdDiscord.Domain.AutoReplies
{
    public record AutoReply(
        string TriggerMessage,
        string ResponseMessage,
        AutoReplyAction AdditionalAction);
}
