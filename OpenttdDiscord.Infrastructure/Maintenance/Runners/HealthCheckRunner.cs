using System.Text;
using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Maintenance.Messages;

namespace OpenttdDiscord.Infrastructure.Maintenance.Runners
{
    internal class HealthCheckRunner : OttdSlashCommandRunnerBase
    {
        public HealthCheckRunner(IAkkaService akkaService,
                                 IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(akkaService, getRoleLevelUseCase)
        {
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Moderator)
                    .ToAsync()
                from selection in AkkaService.SelectActor(MainActors.Paths.HealthCheck)
                from healthCheckResponse in selection.TryAsk<HealthCheckResponse>(
                    new HealthCheckRequest(command.GuildId!.Value))
                select CreateResponse(healthCheckResponse);
        }

        private IInteractionResponse CreateResponse(HealthCheckResponse response)
        {
            StringBuilder sb = new();

            foreach (var kp in response.Entries)
            {
                var key = kp.Key;
                var entry = kp.Value;

                sb.AppendLine($"{key} - {entry.Status} - {entry.Duration.TotalSeconds:0.00} s");
            }

            sb.AppendLine("Servers: ");

            foreach (var s in response.ServersHealthChecks)
            {
                var serverId = s.Key;
                var entry = s.Value;

                sb.AppendLine($"{entry.server.Name} - {entry.HealthStatus} - {entry.CheckTime.TotalSeconds:0.00} s");
            }

            return new TextResponse(sb);
        }
    }
}