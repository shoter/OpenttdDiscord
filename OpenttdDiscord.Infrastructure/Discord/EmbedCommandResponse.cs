using Discord;
using Discord.WebSocket;

namespace OpenttdDiscord.Infrastructure.Discord
{
    public class EmbedCommandResponse : SlashCommandResponseBase
    {
        private readonly Embed embed;

        public EmbedCommandResponse(Embed embed)
        {
            this.embed = embed;
        }

        protected override Task InternalExecute(SocketSlashCommand command)
        {
            return command.RespondAsync(embed: embed);
        }
    }
}
