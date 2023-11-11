using System.Diagnostics.CodeAnalysis;
using Discord;
using LanguageExt;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Modals
{
    [ExcludeFromCodeCoverage]
    internal class SetWelcomeMessageModal : OttdModalBase
    {
        public string InitialWelcomeMessage { get; }
        public string ServerName { get; }

        public SetWelcomeMessageModal(Option<string> initialWelcomeMessage,
                                      string serverName)
            : base(AutoReplyModals.SetWelcomeMessage)
        {
            this.ServerName = serverName;
            this.InitialWelcomeMessage = initialWelcomeMessage
                .IfNone(string.Empty);
        }

        protected override void Configure(ModalBuilder builder)
        {
            builder
                .WithTitle("Set welcome message")
                .AddTextInput(
                    "Server Name",
                    "server-name",
                    value: ServerName)
                .AddTextInput(
                    "Welcome message",
                    "content",
                    TextInputStyle.Paragraph,
                    value: InitialWelcomeMessage);
        }
    }
}