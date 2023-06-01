using System.Text;
using Discord.WebSocket;

namespace OpenttdDiscord.Infrastructure.Discord.Responses
{
    public class TextCommandResponse : SlashCommandResponseBase
    {
        private readonly string response;

        public TextCommandResponse(string response)
        {
            this.response = response;
        }

        public TextCommandResponse(StringBuilder sb)
            : this(sb.ToString())
        {
        }

        protected override async Task InternalExecute(SocketSlashCommand command)
        {
            await command.RespondAsync(response);
        }
    }
}
