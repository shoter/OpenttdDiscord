using System.Diagnostics.CodeAnalysis;
using Discord;
using OpenttdDiscord.Infrastructure.Discord.Modals;
using OpenttdDiscord.Infrastructure.Testing.ModalRunners;

namespace OpenttdDiscord.Infrastructure.Testing.Modals
{
    [ExcludeFromCodeCoverage]
    public class TestModal : OttdModalBase<TestModalRunner>
    {
        public TestModal()
            : base("test-modal")
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