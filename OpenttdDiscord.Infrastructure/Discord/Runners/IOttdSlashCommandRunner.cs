using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Discord.Responses;

namespace OpenttdDiscord.Infrastructure.Discord.Runners
{
    public interface IOttdSlashCommandRunner
    {
        EitherAsync<IError, ISlashCommandResponse> Run(SocketSlashCommand command);
    }
}
