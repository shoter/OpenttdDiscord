using Discord.WebSocket;

namespace OpenttdDiscord.Infrastructure.Discord
{
    public interface IOttdSlashCommandRunner
    {
        Task<EitherString> Run(SocketSlashCommand command);
    }
}
