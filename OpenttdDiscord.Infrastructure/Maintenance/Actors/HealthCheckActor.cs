using Akka.Actor;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenttdDiscord.Infrastructure.Maintenance.Messages;

namespace OpenttdDiscord.Infrastructure.Maintenance.Actors
{
    public class HealthCheckActor : ReceiveActorBase
    {
        private IReadOnlyDictionary<string, HealthReportEntry> currentEntries = new Dictionary<string, HealthReportEntry>();

        protected HealthCheckActor(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public static Props Create(IServiceProvider sp) => Props.Create(() => new HealthCheckActor(sp));

        public void Ready()
        {
            Receive<HealthReport>(ProcessReport);
            Receive<HealthCheckRequest>(_ => Sender.Tell(new HealthCheckResponse(currentEntries)));
        }

        public void ProcessReport(HealthReport report)
        {
            currentEntries = report.Entries;
        }
    }
}