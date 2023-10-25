using Discord.WebSocket;
using LanguageExt;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.Modals;
using OpenttdDiscord.Validation;

namespace OpenttdDiscord.Infrastructure.Discord
{
    public class DiscordModalService : IDiscordModalService
    {
        private readonly ILogger logger;
        private readonly DiscordSocketClient client;
        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<string, IOttdModal> modals = new();

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
                this.modals.Add(
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

            if (!modals.ContainsKey(arg.Data.CustomId))
            {
                arg.RespondAsync(
                    "No response is defined for this modal",
                    ephemeral: true);
            }

            var modal = modals[arg.Data.CustomId];
            var runner = modal.CreateRunner(serviceProvider);


            var _ =
                from _1 in runner.Run(arg)
                select Unit.Default;


            return Task.CompletedTask;
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

        private IModalResponse GenerateErrorResponse(
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
}