using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Discord.ModalResponses;

namespace OpenttdDiscord.Infrastructure.Discord.ModalRunners
{
    public interface IOttdModalRunner
    {
        EitherAsync<IError, IModalResponse> Run(IModalInteraction modalInteraction);
    }
}