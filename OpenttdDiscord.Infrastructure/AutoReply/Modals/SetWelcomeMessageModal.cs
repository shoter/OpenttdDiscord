using Discord;
using OpenttdDiscord.Infrastructure.AutoReply.ModalRunners;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.AutoReply.Modals
{
    internal class SetWelcomeMessageModal : OttdModalBase<SetWelcomeMessageModalRunner>
    {
        public SetWelcomeMessageModal()
            : base("set-welcome-message-modal")
        {
        }

        protected override void Configure(ModalBuilder builder)
        {
            builder
                .WithTitle("Set welcome message")
                .AddTextInput(
                    "Server Id",
                    "server-id")
                .AddTextInput(
                    "Welcome message",
                    "content",
                    TextInputStyle.Paragraph,
                    value: "Twoja stara to kopara i twój stary ją wpierdala");
        }
    }
}