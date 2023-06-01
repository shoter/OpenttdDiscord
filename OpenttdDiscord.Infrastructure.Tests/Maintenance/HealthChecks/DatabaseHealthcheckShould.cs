using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Maintenance.HealthChecks;

namespace OpenttdDiscord.Infrastructure.Tests.Maintenance.HealthChecks
{
    public class DatabaseHealthcheckShould
    {
        private readonly Mock<IOttdServerRepository> ottdServerRepositoryMock = new();

        private readonly IHealthCheck databaseHealthCheck;

        public DatabaseHealthcheckShould()
        {
            databaseHealthCheck = new DatabaseHealthcheck(
                ottdServerRepositoryMock.Object,
                NullLogger<DatabaseHealthcheck>.Instance);
        }

        [Fact]
        public async Task ReturnHealthyStatus_WhenServerResponseIsReturnedImmedietally()
        {
            ottdServerRepositoryMock
                .Setup(x => x.GetAllGuilds())
                .Returns(EitherAsync<IError, List<ulong>>.Right(new List<ulong>()));

            Assert.Equal(
                HealthStatus.Healthy,
                (await databaseHealthCheck.CheckHealthAsync(new HealthCheckContext())).Status);
        }

        [Fact]
        public async Task ReturnDegradedStatus_WhenServerResponseIsNotReturnedASAP()
        {
            ottdServerRepositoryMock
                .Setup(x => x.GetAllGuilds())
                .Callback(
                    () =>
                    {
                        Task.Delay(2.Seconds())
                            .Wait();
                    })
                .Returns(EitherAsync<IError, List<ulong>>.Right(new List<ulong>()));

            Assert.Equal(
                HealthStatus.Degraded,
                (await databaseHealthCheck.CheckHealthAsync(new HealthCheckContext())).Status);
        }

        [Fact]
        public async Task ReturnUnhealthy_InCaseOfError()
        {
            ottdServerRepositoryMock
                .Setup(x => x.GetAllGuilds())
                .Returns(EitherAsync<IError, List<ulong>>.Left(new HumanReadableError("Boom!")));

            Assert.Equal(
                HealthStatus.Unhealthy,
                (await databaseHealthCheck.CheckHealthAsync(new HealthCheckContext())).Status);
        }
    }
}