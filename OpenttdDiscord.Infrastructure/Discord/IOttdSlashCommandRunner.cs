using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Discord
{
    public interface IOttdSlashCommandRunner
    {
        EitherAsync<IError, ISlashCommandResponse> Run(SocketSlashCommand command);
    }
}
