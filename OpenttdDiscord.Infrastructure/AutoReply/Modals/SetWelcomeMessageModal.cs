using Discord;
using LanguageExt;
using OpenttdDiscord.Infrastructure.AutoReply.ModalRunners;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.AutoReply.Modals
{
    internal class SetWelcomeMessageModal : OttdModalBase<SetWelcomeMessageModalRunner>
    {
        private readonly string initialWelcomeMessage;
        public SetWelcomeMessageModal(Option<string> initialWelcomeMessage)
            : base("set-welcome-message-modal")
        {
            this.initialWelcomeMessage = initialWelcomeMessage
                .IfNone(string.Empty);
        }

        protected override void Configure(ModalBuilder builder)
        {
            builder
                .WithTitle("Set welcome message")
                .AddTextInput(
                    "Welcome message",
                    "content",
                    TextInputStyle.Paragraph,
                    value: "Twoja stara to kopara i twój stary ją wpierdala");
        }
    }
}