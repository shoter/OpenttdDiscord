using Discord.WebSocket;
using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Validation;

namespace OpenttdDiscord.Infrastructure.Discord
{
    internal class DiscordInteractionService : IDiscordInteractionService
    {
        private readonly ILogger logger;
        private readonly DiscordSocketClient client;
        private readonly IServiceProvider serviceProvider;
        private readonly ValidationErrorEmbedBuilder validationEmbedBuilder = new();
        private readonly Dictionary<string, IOttdSlashCommand> commands = new();

        public DiscordInteractionService(
            IServiceProvider serviceProvider,
            ILogger<DiscordInteractionService> logger,
            DiscordSocketClient client,
            IEnumerable<IOttdSlashCommand> commands
        )
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.client = client;
            foreach (var c in commands)
            {
                this.commands.Add(
                    c.Name,
                    c);
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
            var existingCommands = (await client.GetGlobalApplicationCommandsAsync())
                .ToDictionary(x => x.Name);

            foreach (var c in commands.Values)
            {
                var props = c.Build();

                if (existingCommands.TryGetValue(
                        c.Name,
                        out var existing))
                {
                    int cCount = props.Options.IsSpecified ? props.Options.Value.Count : 0;
                    int exCount = existing.Options.Count();

                    if (cCount == exCount)
                    {
                        continue;
                    }
                    else
                    {
                        await existing.DeleteAsync();
                        logger.LogError($"Removed {c.Name} due to parameter count mismatch");
                    }
                }

                try
                {
                    logger.LogInformation($"Registering {c}");
                    await client.CreateGlobalApplicationCommandAsync(props);
                    logger.LogInformation($"Registered {c}");
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        $"Something when wrong while registering {c}");
                }
            }
        }

        private async Task Client_SlashCommandExecuted(SocketSlashCommand arg)
        {
            logger.LogDebug(
                "{0} executing {1}",
                arg.User.Username,
                arg.CommandName);
            var command = this.commands[arg.Data.Name];
            using var scope = serviceProvider.CreateScope();
            var runner = command.CreateRunner(scope.ServiceProvider);
            logger.LogDebug("Created runner");

            Task<IInteractionResponse> responseTask = GetSlashCommandResponse(
                arg,
                runner);
            Task timeoutTask = Task.Delay(
                2.Seconds()
                    .ToTimeSpan());

            await Task.WhenAny(
                timeoutTask,
                responseTask);

            if (responseTask.IsCompletedSuccessfully)
            {
                await ExecuteResponse(
                    arg,
                    responseTask.Result);
            }
            else
            {
                await ExecuteResponse(
                    arg,
                    new TextResponse("Command has time outed :("));
            }
        }

        private async Task<IInteractionResponse> GetSlashCommandResponse(
            SocketSlashCommand arg,
            IOttdSlashCommandRunner runner)
        {
            var response = (await runner.Run(arg))
                .IfLeft(
                    err => GenerateErrorResponse(
                        err,
                        arg));
            return response;
        }

        private async Task ExecuteResponse(
            SocketSlashCommand arg,
            IInteractionResponse response)
        {
            (await response.Execute(arg))
                .MapLeft(
                    (IError error) =>
                    {
                        if (error is ExceptionError ee)
                        {
                            logger.LogError(
                                ee.Exception,
                                $"Something went wrong while executing some command {arg.CommandName}.");
                        }

                        logger.LogWarning(
                            $"{arg.User.Username} executed unsuccessfully {arg.CommandName} - {error.Reason}");
                        return error;
                    })
                .Map(
                    unit =>
                    {
                        logger.LogInformation($"{arg.User.Username} executed successfully {arg.CommandName}");
                        return unit;
                    });
        }

        private IInteractionResponse GenerateErrorResponse(
            IError error,
            SocketSlashCommand arg)
        {
            string text =
                error is HumanReadableError ? $"Error: {error.Reason}" : "Something went wrong :(";

            if (error is ExceptionError ee)
            {
                logger.LogError(
                    ee.Exception,
                    $"Something went wrong while executing some command {arg.CommandName}.");
            }

            if (error is ValidationError ve)
            {
                return new EmbedResponse(validationEmbedBuilder.BuildEmbed(ve));
            }

            return new TextResponse(text);
        }
    }
}