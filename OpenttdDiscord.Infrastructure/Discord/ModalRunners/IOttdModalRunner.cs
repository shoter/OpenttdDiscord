using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Discord.ModalResponses;
using OpenttdDiscord.Infrastructure.Discord.Responses;

namespace OpenttdDiscord.Infrastructure.Discord.ModalRunners
{
    public interface IOttdModalRunner
    {
        EitherAsync<IError, IModalResponse> Run(IModalInteraction modalInteraction);
    }
}