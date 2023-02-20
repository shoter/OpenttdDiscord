using Discord;
using Discord.WebSocket;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenttdDiscord.Base.Discord;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Discord.Options;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Validation;
using Serilog.Core;
using System.Collections.Generic;

namespace OpenttdDiscord.Discord.Services
{
    internal class DiscordService : BackgroundService
    {
        private readonly DiscordSocketClient client;
        private readonly ILogger logger;
        private readonly DiscordOptions options;
        private readonly IDiscordCommandService discordCommandService;

        public DiscordService(
            DiscordSocketClient client,
            ILogger<DiscordService> logger,
            IDiscordCommandService discordCommandService,
            IOptions<DiscordOptions> options)
        {
            this.client = client;
            this.logger = logger;
            this.discordCommandService = discordCommandService;
            this.options = options.Value;
            client.Log += OnDiscordLog;
            client.Ready += Client_Ready;
        }

     

        private async Task Client_Ready()
        {
            await this.discordCommandService.Register();
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
