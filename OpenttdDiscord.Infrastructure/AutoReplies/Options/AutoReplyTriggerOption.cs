using System.Diagnostics.CodeAnalysis;
using Discord;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Options
{
    [ExcludeFromCodeCoverage]
    public class AutoReplyTriggerOption : SlashCommandOptionBuilder
    {
        public const string OptionName = "trigger-message";
        public AutoReplyTriggerOption()
        {
            WithName(OptionName)
                .WithDescription("Trigger message of auto reply")
                .WithType(ApplicationCommandOptionType.String);
        }
    }
}