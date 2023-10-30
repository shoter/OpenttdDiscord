using Discord;
using LanguageExt;
using OpenttdDiscord.Infrastructure.AutoReply.ModalRunners;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.AutoReply.Modals
{
    internal class SetWelcomeMessageModal : OttdModalBase<SetWelcomeMessageModalRunner>
    {
        public string InitialWelcomeMessage { get; }
        public string ServerName { get; }

        public SetWelcomeMessageModal(Option<string> initialWelcomeMessage,
                                      string serverName)
            : base("set-welcome-message-modal")
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
                    InitialWelcomeMessage,
                    TextInputStyle.Paragraph,
                    value: "Twoja stara to kopara i twój stary ją wpierdala");
        }
    }
}