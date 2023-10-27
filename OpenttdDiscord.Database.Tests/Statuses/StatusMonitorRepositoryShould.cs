using System.Runtime.CompilerServices;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Statuses
{
    public class StatusMonitorRepositoryShould : DatabaseBaseTest
    {
        public StatusMonitorRepositoryShould(PostgressDatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
        }

        [Fact]
        public async Task InsertMonitorToDatabase()
        {
            var repository = await CreateRpeository();
            var server = await CreateServer();
            var expectedMonitor = Fix.Create<StatusMonitor>();
            expectedMonitor = expectedMonitor with
            {
                ServerId = server.Id,
                LastUpdateTime = expectedMonitor.LastUpdateTime.ToUniversalTime()
            };

            (await repository.Insert(expectedMonitor)).ThrowIfError();
            List<StatusMonitor> retrievedMonitors = (await repository.GetStatusMonitors(expectedMonitor.ServerId))
                .ThrowIfError()
                .Right();

            Assert.Single(retrievedMonitors);
            Equal(expectedMonitor, retrievedMonitors.First());
        }

        [Fact]
        public async Task RemoveMonitorFromDatabase()
        {
            var repository = await CreateRpeository();
            var server = await CreateServer();
            var expectedMonitor = Fix.Create<StatusMonitor>();
            expectedMonitor = expectedMonitor with
            {
                ServerId = server.Id,
                LastUpdateTime = expectedMonitor.LastUpdateTime.ToUniversalTime()
            };

            List<StatusMonitor> retrievedMonitors =
            (await (
                from _1 in repository.Insert(expectedMonitor)
                from _2 in repository.RemoveStatusMonitor(server.Id, expectedMonitor.ChannelId)
                from monitors in repository.GetStatusMonitors(expectedMonitor.ServerId)
                select monitors
            ))
            .ThrowIfError()
            .Right();

            Assert.Empty(retrievedMonitors);
        }

        [Fact]
        public async Task UpdateMonitorInDatabase()
        {
            var repository = await CreateRpeository();
            var server = await CreateServer();
            var initialMonitor = Fix.Create<StatusMonitor>();
            initialMonitor = initialMonitor with
            {
                ServerId = server.Id,
                LastUpdateTime = initialMonitor.LastUpdateTime.ToUniversalTime()
            };

            var updatedMonitor = initialMonitor with
            {
                MessageId = 2137,
                LastUpdateTime = DateTime.UtcNow
            };

            List<StatusMonitor> retrievedMonitors = (await
            (from _1 in repository.Insert(initialMonitor)
             from _2 in repository.UpdateStatusMonitor(updatedMonitor)
             from monitors in repository.GetStatusMonitors(initialMonitor.ServerId)
             select monitors
            ))
            .ThrowIfError()
            .Right();

            Assert.Single(retrievedMonitors);
            Equal(updatedMonitor, retrievedMonitors.First());
        }

        private async Task<StatusMonitorRepository> CreateRpeository([CallerMemberName] string? databaseName = null)
        {
            var context = await CreateContext(databaseName);
            return new StatusMonitorRepository(context);
        }

        private async Task<OttdServer> CreateServer([CallerMemberName] string? databaseName = null)
        {
            var context = await CreateContext(databaseName);
            var repository = new OttdServerRepository(context);
            var server = Fix.Create<OttdServer>();
            (await repository.InsertServer(server)).ThrowIfError();
            return server;
        }

        private void Equal(StatusMonitor lhm, StatusMonitor rhm)
        {
            Assert.Equal(lhm.ServerId, rhm.ServerId);
            Assert.Equal(lhm.ChannelId, rhm.ChannelId);
            Assert.Equal(lhm.MessageId, rhm.MessageId);
            Assert.Equal(lhm.LastUpdateTime, rhm.LastUpdateTime, TimeSpan.FromMilliseconds(1));
        }
    }
}
