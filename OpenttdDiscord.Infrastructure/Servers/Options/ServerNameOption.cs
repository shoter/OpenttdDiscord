using System.Diagnostics.CodeAnalysis;
using Discord;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Servers.Options
{
    [ExcludeFromCodeCoverage]
    public class ServerNameOption : SlashCommandOptionBuilder
    {
        public const string OptionName = "server-name";

        public ServerNameOption()
        {
            WithName(OptionName)
                .WithDescription("Server name")
                .WithType(ApplicationCommandOptionType.String);
        }

        public static string GetValue(OptionsDictionary dictionary) => dictionary.GetValueAs<string>(OptionName);
    }
}