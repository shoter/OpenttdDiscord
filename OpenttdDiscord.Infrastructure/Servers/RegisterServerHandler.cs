using Discord.WebSocket;
using OpenttdDiscord.Infrastructure.Discord;

namespace OpenttdDiscord.Infrastructure.Servers
{
    internal class RegisterServerHandler : IOttdSlashCommandRunner
    {
        public async Task Run(SocketSlashCommand command)
        {
            await command.RespondAsync($"You executed {command.Data.Name}");
        }
    }
}
