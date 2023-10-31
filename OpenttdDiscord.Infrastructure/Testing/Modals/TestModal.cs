using System.Diagnostics.CodeAnalysis;
using Discord;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.Testing.Modals
{
    [ExcludeFromCodeCoverage]
    public class TestModal : OttdModalBase
    {
        public TestModal()
            : base(TestModals.TestModal)
        {
        }

        protected override void Configure(ModalBuilder builder)
        {
            builder
                .WithTitle("This is a test modal")
                .AddTextInput(
                    "Label",
                    "labelId",
                    TextInputStyle.Short);
        }
    }
}