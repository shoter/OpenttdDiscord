using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Validation;

namespace OpenttdDiscord.Infrastructure.Discord
{
    internal class DiscordCommandService : IDiscordCommandService
    {
        private readonly ILogger logger;
        private readonly DiscordSocketClient client;
        private readonly IServiceProvider serviceProvider;
        private readonly ValidationErrorEmbedBuilder validationEmbedBuilder = new();
        private readonly Dictionary<string, IOttdSlashCommand> commands = new();

        public DiscordCommandService(
            IServiceProvider serviceProvider,
            ILogger<DiscordCommandService> logger,
            DiscordSocketClient client,
            IEnumerable<IOttdSlashCommand> commands
            )
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.client = client;
                        foreach(var c in commands)
            {
                this.commands.Add(c.Name, c);
            }
}

        public async Task Register()
        {
            client.SlashCommandExecuted += Client_SlashCommandExecuted;

            await RegisterCommands();
            await PruneCommands();
        }

        private async Task PruneCommands()
        {
            var existingCommands = await client.GetGlobalApplicationCommandsAsync();

            foreach (var c in existingCommands)
            {
                string name = c.Name;
                if (commands.ContainsKey(name))
                {
                    continue;
                }

                logger.LogInformation($"Removing dangling command {name}");
                await c.DeleteAsync();
                logger.LogInformation($"{name} deleted!");
            }
        }

        private async Task RegisterCommands()
        {
            foreach (var c in commands.Values)
            {
                try
                {
                    var parameters = c.Build();
                    await client.CreateGlobalApplicationCommandAsync(parameters);
                    logger.LogInformation($"Registered {c}");

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Something when wrong while registering {c}");
                }
            }
        }

        private async Task Client_SlashCommandExecuted(SocketSlashCommand arg)
        {
            var command = this.commands[arg.Data.Name];

            using var scope = serviceProvider.CreateScope();
            var runner = command.CreateRunner(scope.ServiceProvider);
            var response = (await runner.Run(arg))
                           .IfLeft(err => GenerateErrorResponse(err, arg));

            (await response.Execute(arg))
                .IfLeft((IError error) =>
                {
                    if (error is ExceptionError ee)
                    {
                        logger.LogError(ee.Exception, $"Something went wrong while executing some command {arg.CommandName}.");
                    }
                });
        }

        private ISlashCommandResponse GenerateErrorResponse(IError error, SocketSlashCommand arg)
        {
            string text =
                error is HumanReadableError ?
                $"Error: {error.Reason}" :
                "Something went wrong :(";

            if (error is ExceptionError ee)
            {
                logger.LogError(ee.Exception, $"Something went wrong while executing some command {arg.CommandName}.");
            }

            if (error is ValidationError ve)
            {
                return new EmbedCommandResponse(validationEmbedBuilder.BuildEmbed(ve));
            }

            return new TextCommandResponse(text);
        }



    }
}
