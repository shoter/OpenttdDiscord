using System.Diagnostics.CodeAnalysis;
using Discord;

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
    }
}