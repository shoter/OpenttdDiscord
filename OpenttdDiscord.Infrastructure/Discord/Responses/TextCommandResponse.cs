using System.Text;
using Discord;
using Discord.WebSocket;

namespace OpenttdDiscord.Infrastructure.Discord.Responses
{
    public class TextCommandResponse : SlashCommandResponseBase
    {
        private readonly string response;

        public TextCommandResponse(string response)
        {
            this.response = response;
            if (string.IsNullOrWhiteSpace(this.response))
            {
                this.response = "Empty response";
            }
        }

        public TextCommandResponse(StringBuilder sb)
            : this(sb.ToString())
        {
        }

        protected override async Task InternalExecute(ISlashCommandInteraction command)
        {
            await command.RespondAsync(response);
        }
    }
}
