using System.Text;
using Discord;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public class TextResponse : InteractionResponseBase
    {
        private readonly string response;

        private readonly bool ephemeral;

        public TextResponse(string response, bool ephemeral = true)
        {
            this.response = response;
            this.ephemeral = ephemeral;
            if (string.IsNullOrWhiteSpace(this.response))
            {
                this.response = "Empty response";
            }
        }

        public TextResponse(StringBuilder sb, bool ephemeral = true)
            : this(sb.ToString(), ephemeral)
        {
        }

        protected override async Task InternalExecute(ISlashCommandInteraction command)
        {
            await command.RespondAsync(response, ephemeral: ephemeral);
        }
    }
}
