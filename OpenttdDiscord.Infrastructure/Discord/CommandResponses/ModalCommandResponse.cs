using Discord;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public class ModalCommandResponse : SlashCommandResponseBase
    {
        private readonly IOttdModal modal;

        public ModalCommandResponse(IOttdModal modal)
        {
            this.modal = modal;
        }

        protected override Task InternalExecute(ISlashCommandInteraction command)
        {
            return command.RespondWithModalAsync(modal.Build());
        }
    }
}