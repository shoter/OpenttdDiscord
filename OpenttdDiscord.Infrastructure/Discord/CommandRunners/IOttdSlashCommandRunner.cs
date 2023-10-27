using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;

namespace OpenttdDiscord.Infrastructure.Discord.CommandRunners
{
    public interface IOttdSlashCommandRunner
    {
        EitherAsync<IError, IInteractionResponse> Run(ISlashCommandInteraction command);
    }
}
