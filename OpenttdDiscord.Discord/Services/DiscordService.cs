using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenttdDiscord.Base.Discord;
using OpenttdDiscord.Discord.Options;
using OpenttdDiscord.Infrastructure.Discord;
using Serilog.Core;
using System.Collections.Generic;

namespace OpenttdDiscord.Discord.Services
{
    internal class DiscordService : BackgroundService
    {
        private readonly DiscordSocketClient client = new();

        private readonly ILogger logger;
        private readonly DiscordOptions options;
        private readonly Dictionary<string, IOttdSlashCommand> commands = new();
        private readonly IServiceProvider sp;

        public DiscordService(
            ILogger<DiscordService> logger,
            IEnumerable<IOttdSlashCommand> commands,
            IServiceProvider sp,
            IOptions<DiscordOptions> options)
        {
            this.logger = logger;
            client.Log += OnDiscordLog;
            client.Ready += Client_Ready;
            client.SlashCommandExecuted += Client_SlashCommandExecuted;
            this.options = options.Value;
            this.sp = sp;
            foreach(var c in commands)
            {
                this.commands.Add(c.Name, c);
            }
        }

        private async Task Client_SlashCommandExecuted(SocketSlashCommand arg)
        {
            var command = this.commands[arg.Data.Name];

            using var scope = sp.CreateScope();
            var runner = command.CreateRunner(scope.ServiceProvider);
            await runner.Run(arg);
        }

        private async Task Client_Ready()
        {
            foreach(var c in commands.Values)
            {
                try
                {
                    var parameters = c.Build();
                    await client.CreateGlobalApplicationCommandAsync(parameters);
                    logger.LogInformation($"Registered {c}");
                    
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, $"Something when wrong while registering {c}");
                }
            }
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
