using System.Text;
using Discord;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public class TextResponse : InteractionResponseBase
    {
        public string Response { get; }

        private readonly bool ephemeral;

        public TextResponse(string response, bool ephemeral = true)
        {
            this.Response = response;
            this.ephemeral = ephemeral;
            if (string.IsNullOrWhiteSpace(this.Response))
            {
                this.Response = "Empty response";
            }
        }

        public TextResponse(StringBuilder sb, bool ephemeral = true)
            : this(sb.ToString(), ephemeral)
        {
        }

        protected override async Task InternalExecute(IDiscordInteraction interaction)
        {
            await interaction.RespondAsync(Response, ephemeral: ephemeral);
        }
    }
}
