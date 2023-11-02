namespace OpenttdDiscord.Domain.AutoReplies
{
    public record AutoReply(string TriggerMessage,
                            string ResponseMessage,
                            AutoReplyAction AdditionalAction)
    {
        public string TriggerMessage { get; set; } = TriggerMessage;
        public string ResponseMessage { get; set; } = ResponseMessage;
        public AutoReplyAction AdditionalAction { get; set; } = AdditionalAction;
    }
}
