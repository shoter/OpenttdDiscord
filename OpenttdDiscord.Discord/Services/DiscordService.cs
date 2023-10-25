using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenttdDiscord.Base.Discord;
using OpenttdDiscord.Discord.Options;
using OpenttdDiscord.Infrastructure.Discord;

namespace OpenttdDiscord.Discord.Services
{
    internal class DiscordService : BackgroundService
    {
        private readonly DiscordSocketClient client;
        private readonly ILogger logger;
        private readonly DiscordOptions options;
        private readonly IDiscordCommandService discordCommandService;
        private readonly IDiscordModalService discordModalService;

        public DiscordService(
            DiscordSocketClient client,
            ILogger<DiscordService> logger,
            IDiscordCommandService discordCommandService,
            IOptions<DiscordOptions> options,
            IDiscordModalService discordModalService)
        {
            this.client = client;
            this.logger = logger;
            this.discordCommandService = discordCommandService;
            this.discordModalService = discordModalService;
            this.options = options.Value;
            client.Log += OnDiscordLog;
            client.Ready += Client_Ready;
        }

        private async Task Client_Ready()
        {
            await this.discordCommandService.Register();
            await this.discordModalService.Register();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await client.LoginAsync(TokenType.Bot, options.Token);
            await client.StartAsync();
        }

        private Task OnDiscordLog(LogMessage logMessage)
        {
            logger.Log(logMessage.Severity.ToLogLevel(), logMessage.Exception, logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
