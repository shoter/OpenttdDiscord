using Discord;
using Discord.WebSocket;

namespace OpenttdDiscord.Infrastructure.Discord.Responses
{
    public interface ISlashCommandResponse
    {
        public Task<EitherUnit> Execute(ISlashCommandInteraction command);
    }
}
