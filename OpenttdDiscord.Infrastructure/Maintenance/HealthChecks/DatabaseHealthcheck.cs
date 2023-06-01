using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Maintenance.HealthChecks
{
    public class DatabaseHealthcheck : IHealthCheck
    {
        private readonly IOttdServerRepository serverRepository;
        private readonly ILogger logger;

        public DatabaseHealthcheck(
            IOttdServerRepository serverRepository,
            ILogger<DatabaseHealthcheck> logger)
        {
            this.serverRepository = serverRepository;
            this.logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Stopwatch stopWatch = new();
                stopWatch.Start();
                (await serverRepository.GetAllGuilds()).ThrowIfError();
                stopWatch.Stop();

                var time = stopWatch.Elapsed;

                if (time.TotalSeconds > 1.0)
                {
                    return HealthCheckResult.Degraded("Response time for SQL query was longer than 1 second");
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Something when wrong during database health check");
                return HealthCheckResult.Unhealthy(
                    "Something when wrong with database health check",
                    ex);
            }
        }
    }
}