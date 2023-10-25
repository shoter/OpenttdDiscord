using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;

namespace OpenttdDiscord.Infrastructure.Discord.ModalRunners
{
    public interface IOttdModalRunner
    {
        EitherAsync<IError, IInteractionResponse> Run(IModalInteraction modalInteraction);
    }
}