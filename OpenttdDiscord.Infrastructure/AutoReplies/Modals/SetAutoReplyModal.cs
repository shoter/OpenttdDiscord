using Discord;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Modals
{
    public class SetAutoReplyModal : OttdModalBase
    {
        public string ServerName { get; }
        public string TriggerMessage { get; }
        public string ResponseMessage { get; }
        public string AdditionalAction { get; }

        public SetAutoReplyModal(
            string serverName,
            string additionalAction,
            string triggerMessage,
            Option<string> responseMessage)
            : base(AutoReplyModals.SetAutoReply)
        {
            this.ServerName = serverName;
            AdditionalAction = additionalAction;
            TriggerMessage = triggerMessage;
            this.ResponseMessage = responseMessage
                .IfNone(string.Empty);
        }

        protected override void Configure(ModalBuilder builder)
        {
            builder
                .WithTitle("Set auto reply")
                .AddTextInput(
                    "Server Name",
                    "server-name",
                    value: ServerName,
                    required: true)
                .AddTextInput(
                    "Action",
                    "action",
                    TextInputStyle.Paragraph,
                    value: AdditionalAction,
                    required: true)
                .AddTextInput(
                    "Trigger message",
                    "trigger",
                    TextInputStyle.Paragraph,
                    value: TriggerMessage,
                    required: true)
                .AddTextInput(
                    "Auto reply message",
                    "content",
                    TextInputStyle.Paragraph,
                    value: ResponseMessage,
                    required: true);
        }
    }
}