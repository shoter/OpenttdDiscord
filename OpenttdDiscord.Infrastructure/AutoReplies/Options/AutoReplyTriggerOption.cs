using System.Diagnostics.CodeAnalysis;
using Discord;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

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

        public static string GetValue(OptionsDictionary options) => options.GetValueAs<string>(OptionName);
    }
}