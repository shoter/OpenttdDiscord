using LanguageExt;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Akkas;

namespace OpenttdDiscord.Infrastructure.Maintenance
{
    public class HealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly IAkkaService akkaService;

        public HealthCheckPublisher(IAkkaService akkaService)
        {
            this.akkaService = akkaService;
        }

        public async Task PublishAsync(
            HealthReport report,
            CancellationToken cancellationToken)
        {
            var either =
                await (from selection in akkaService.SelectActor(MainActors.Paths.HealthCheck)
                    from _1 in selection.TellExt(report)
                        .ToAsync()
                    select Unit.Default);

            either.ThrowIfError();
        }
    }
}