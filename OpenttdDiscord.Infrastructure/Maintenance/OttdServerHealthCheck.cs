using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Maintenance
{
    public record OttdServerHealthCheck(
        DateTimeOffset HealthCheckTime,
        OttdServer server,
        TimeSpan CheckTime,
        HealthStatus HealthStatus
    );
}