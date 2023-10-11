using System.Text;
using Discord;

namespace OpenttdDiscord.Infrastructure.Discord.ModalResponses
{
    public class ModalTextResponse : ModalResponseBase
    {
        private readonly string response;

        private readonly bool ephemeral;

        public ModalTextResponse(
            string response,
            bool ephemeral = true)
        {
            this.response = response;
            this.ephemeral = ephemeral;
            if (string.IsNullOrWhiteSpace(this.response))
            {
                this.response = "Empty response";
            }
        }

        public ModalTextResponse(
            StringBuilder sb,
            bool ephemeral = true)
            : this(
                sb.ToString(),
                ephemeral)
        {
        }

        protected override async Task InternalExecute(IModalInteraction modal)
        {
            await modal.RespondAsync(
                response,
                ephemeral: ephemeral);
        }
    }
}