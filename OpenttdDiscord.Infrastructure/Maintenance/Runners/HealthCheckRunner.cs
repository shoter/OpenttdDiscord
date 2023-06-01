using System.Text;
using Akka.Actor;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;
using OpenttdDiscord.Infrastructure.Maintenance.Messages;

namespace OpenttdDiscord.Infrastructure.Maintenance.Runners
{
    internal class HealthCheckRunner : OttdSlashCommandRunnerBase
    {
        private readonly IAkkaService akkaService;

        public HealthCheckRunner(IAkkaService akkaService)
        {
            this.akkaService = akkaService;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            SocketSlashCommand command,
            User user,
            ExtDictionary<string, object> options)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Moderator)
                    .ToAsync()
                from selection in akkaService.SelectActor(MainActors.Paths.HealthCheck)
                from healthCheckResponse in selection.TryAsk<HealthCheckResponse>(new HealthCheckRequest())
                select CreateResponse(healthCheckResponse);
        }

        private ISlashCommandResponse CreateResponse(HealthCheckResponse response)
        {
            StringBuilder sb = new();

            foreach (var kp in response.Entries)
            {
                var key = kp.Key;
                var entry = kp.Value;

                sb.AppendLine($"{key} - {entry.Status} - {entry.Duration.TotalSeconds:0.00} s");
            }

            return new TextCommandResponse(sb);
        }
    }
}