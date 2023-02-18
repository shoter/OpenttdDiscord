using Discord.WebSocket;

namespace OpenttdDiscord.Infrastructure.Discord
{
    public interface ISlashCommandResponse
    {
        public Task<EitherUnit> Execute(SocketSlashCommand command);
    }
}
