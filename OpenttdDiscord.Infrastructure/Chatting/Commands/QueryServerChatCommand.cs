using Discord;
using OpenttdDiscord.Infrastructure.Chatting.Runners;
using OpenttdDiscord.Infrastructure.Discord.Commands;

namespace OpenttdDiscord.Infrastructure.Chatting.Commands
{
    internal class QueryServerChatCommand : OttdSlashCommandBase<QueryServerChatRunner>
    {
        public QueryServerChatCommand()
            : base("query-server-chat")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Returns file with latest messages from server chat")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
