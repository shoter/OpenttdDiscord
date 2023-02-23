using AutoFixture;
using OpenttdDiscord.Domain.Servers;
using System.Threading.Tasks;
using System;
using Xunit;
using OpenttdDiscord.Database.Servers;
using System.Runtime.CompilerServices;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Database.Statuses;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using System.Collections.Generic;
using System.Linq;

namespace OpenttdDiscord.Database.Tests.Statuses
{
    public class StatusMonitorRepositoryShould : DatabaseBaseTest
    {
        public StatusMonitorRepositoryShould(PostgressDatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task InsertMonitorToDatabase()
        {
            var repository = await CreateRpeository();
            var server = await CreateServer();
            var expectedMonitor = fix.Create<StatusMonitor>();
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
            Assert.Equal(expectedMonitor, retrievedMonitors.First());
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
            var server = fix.Create<OttdServer>();
            (await repository.InsertServer(server)).ThrowIfError();
            return server;

        }
    }
}
