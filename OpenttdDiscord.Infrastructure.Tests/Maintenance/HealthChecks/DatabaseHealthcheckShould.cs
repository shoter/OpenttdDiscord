using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute.Core;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Maintenance.HealthChecks;

namespace OpenttdDiscord.Infrastructure.Tests.Maintenance.HealthChecks
{
    public class DatabaseHealthcheckShould
    {
        private readonly IOttdServerRepository ottdServerRepositoryMock = Substitute.For<IOttdServerRepository>();

        private readonly IHealthCheck databaseHealthCheck;

        public DatabaseHealthcheckShould()
        {
            databaseHealthCheck = new DatabaseHealthcheck(
                ottdServerRepositoryMock,
                NullLogger<DatabaseHealthcheck>.Instance);
        }

        [Fact]
        public async Task ReturnHealthyStatus_WhenServerResponseIsReturnedImmedietally()
        {
            ottdServerRepositoryMock
                .GetAllGuilds()
                .Returns(EitherAsync<IError, List<ulong>>.Right(new List<ulong>()));

            Assert.Equal(
                HealthStatus.Healthy,
                (await databaseHealthCheck.CheckHealthAsync(new HealthCheckContext())).Status);
        }

        [Fact]
        public async Task ReturnDegradedStatus_WhenServerResponseIsNotReturnedASAP()
        {
            ottdServerRepositoryMock
                .GetAllGuilds()
                .Returns(_ =>
                {
                    Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                    return EitherAsync<IError, List<ulong>>.Right(new List<ulong>());
                });

            Assert.Equal(
                HealthStatus.Degraded,
                (await databaseHealthCheck.CheckHealthAsync(new HealthCheckContext())).Status);
        }

        [Fact]
        public async Task ReturnUnhealthy_InCaseOfError()
        {
            ottdServerRepositoryMock
                .GetAllGuilds()
                .Returns(EitherAsync<IError, List<ulong>>.Left(new HumanReadableError("Boom!")));

            Assert.Equal(
                HealthStatus.Unhealthy,
                (await databaseHealthCheck.CheckHealthAsync(new HealthCheckContext())).Status);
        }
    }
}