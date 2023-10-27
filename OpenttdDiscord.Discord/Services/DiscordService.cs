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
        private readonly IDiscordInteractionService discordInteractionService;

        public DiscordService(
            DiscordSocketClient client,
            ILogger<DiscordService> logger,
            IDiscordInteractionService discordInteractionService,
            IOptions<DiscordOptions> options)
        {
            this.client = client;
            this.logger = logger;
            this.discordInteractionService = discordInteractionService;
            this.options = options.Value;
            client.Log += OnDiscordLog;
            client.Ready += Client_Ready;
        }

        private async Task Client_Ready()
        {
            await this.discordInteractionService.Register();
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
