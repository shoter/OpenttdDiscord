using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OpenttdDiscord.Infrastructure.Maintenance
{
    public record OttdServerHealthCheck(
        DateTimeOffset HealthCheckTime,
        Guid ServerId,
        ulong GuildId,
        TimeSpan CheckTime,
        HealthStatus HealthStatus
    );
}