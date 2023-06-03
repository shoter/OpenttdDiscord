using Akka.Actor;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenttdDiscord.Infrastructure.Maintenance.Messages;

namespace OpenttdDiscord.Infrastructure.Maintenance.Actors
{
    public class HealthCheckActor : ReceiveActorBase
    {
        private readonly IDictionary<
            ulong,
            Dictionary<Guid, OttdServerHealthCheck>
        > guildServerEntries = new Dictionary<ulong, Dictionary<Guid, OttdServerHealthCheck>>();

        private IReadOnlyDictionary<string, HealthReportEntry> currentEntries = new Dictionary<string, HealthReportEntry>();

        public HealthCheckActor(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            Ready();
        }

        public static Props Create(IServiceProvider sp) => Props.Create(() => new HealthCheckActor(sp));

        public void Ready()
        {
            Receive<HealthReport>(ProcessReport);
            Receive<HealthCheckRequest>(HealthCheckRequest);
            Receive<OttdServerHealthCheck>(OttdServerHealthCheck);
        }

        public void ProcessReport(HealthReport report)
        {
            currentEntries = report.Entries;
        }

        public void HealthCheckRequest(HealthCheckRequest req)
        {
            IReadOnlyDictionary<Guid, OttdServerHealthCheck> ottdServerHealthChecks =
                new Dictionary<Guid, OttdServerHealthCheck>();

            if (guildServerEntries.ContainsKey(req.GuildId))
            {
                ottdServerHealthChecks = guildServerEntries[req.GuildId];
            }

            Sender.Tell(new HealthCheckResponse(currentEntries, ottdServerHealthChecks));
        }

        public void OttdServerHealthCheck(OttdServerHealthCheck check)
        {
            ulong guildId = check.server.GuildId;

            if (!guildServerEntries.ContainsKey(guildId))
            {
                guildServerEntries[guildId] = new Dictionary<Guid, OttdServerHealthCheck>();
            }

            guildServerEntries[guildId][check.server.Id] = check;
        }
    }
}