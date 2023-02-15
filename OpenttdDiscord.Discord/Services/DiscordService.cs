using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenttdDiscord.Base.Discord;
using OpenttdDiscord.Discord.Options;
using Serilog.Core;

namespace OpenttdDiscord.Discord.Services
{
    internal class DiscordService : BackgroundService
    {
        private readonly DiscordSocketClient client = new();

        private readonly ILogger logger;
        private readonly DiscordOptions options;

        public DiscordService(ILogger<DiscordService> logger, IOptions<DiscordOptions> options)
        {
            this.logger = logger;
            client.Log += OnDiscordLog;
            this.options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("I am hosted service which was run :)");
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
