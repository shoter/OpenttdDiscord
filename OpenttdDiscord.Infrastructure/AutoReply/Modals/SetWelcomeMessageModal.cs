using Discord;
using LanguageExt;
using OpenttdDiscord.Infrastructure.AutoReply.ModalRunners;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.AutoReply.Modals
{
    internal class SetWelcomeMessageModal : OttdModalBase<SetWelcomeMessageModalRunner>
    {
        private readonly string initialWelcomeMessage;
        private readonly string serverName;

        public SetWelcomeMessageModal(Option<string> initialWelcomeMessage,
                                      string serverName)
            : base("set-welcome-message-modal")
        {
            this.serverName = serverName;
            this.initialWelcomeMessage = initialWelcomeMessage
                .IfNone(string.Empty);
        }

        protected override void Configure(ModalBuilder builder)
        {
            builder
                .WithTitle("Set welcome message")
                .AddTextInput(
                    "Server Name",
                    "server-name",
                    value: serverName)
                .AddTextInput(
                    "Welcome message",
                    "content",
                    TextInputStyle.Paragraph,
                    value: "Twoja stara to kopara i twój stary ją wpierdala");
        }
    }
}