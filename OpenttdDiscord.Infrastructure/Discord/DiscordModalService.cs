using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Discord.Modals;

namespace OpenttdDiscord.Infrastructure.Discord
{
    public class DiscordModalService
    {
        private readonly ILogger logger;
        private readonly DiscordSocketClient client;
        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<string, IOttdModal> commands = new();

        public DiscordModalService(
            IServiceProvider serviceProvider,
            ILogger<DiscordModalService> logger,
            DiscordSocketClient client,
            IEnumerable<IOttdModal> modals
        )
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.client = client;
            foreach (var m in modals)
            {
                this.commands.Add(
                    m.Id,
                    m);
            }
        }

        public Task Register()
        {
            client.ModalSubmitted += ModalSubmitted;
            return Task.CompletedTask;
        }

        private Task ModalSubmitted(SocketModal arg)
        {
            logger.LogDebug(
                "{0} responded to {1}",
                arg.User.Username,
                arg.Data.CustomId);

            return Task.CompletedTask;
        }
    }
}