using Discord;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public class ModalResponse : InteractionResponseBase
    {
        public IOttdModal Modal { get; }

        public ModalResponse(IOttdModal modal)
        {
            this.Modal = modal;
        }

        protected override Task InternalExecute(IDiscordInteraction interaction)
        {
            return interaction.RespondWithModalAsync(Modal.Build());
        }
    }
}