using Discord;

namespace OpenttdDiscord.Infrastructure.Discord.ModalResponses
{
    public interface IModalResponse
    {
        EitherAsyncUnit Execute(IModalInteraction modal);
    }
}