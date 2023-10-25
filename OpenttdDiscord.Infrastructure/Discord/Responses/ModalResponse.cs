using Discord;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public class ModalResponse : InteractionResponseBase
    {
        private readonly IOttdModal modal;

        public ModalResponse(IOttdModal modal)
        {
            this.modal = modal;
        }

        protected override Task InternalExecute(IDiscordInteraction interaction)
        {
            return interaction.RespondWithModalAsync(modal.Build());
        }
    }
}