using Discord.WebSocket;

namespace OpenttdDiscord.Infrastructure.Discord
{
    public interface IOttdSlashCommandRunner
    {
        Task Run(SocketSlashCommand command);
    }
}
