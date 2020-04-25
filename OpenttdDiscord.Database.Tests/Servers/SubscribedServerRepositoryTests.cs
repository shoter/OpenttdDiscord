using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Testing;
using OpenttdDiscord.Testing.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Servers
{
    public class SubscribedServerRepositoryTests : MysqlTest
    {
        public SubscribedServerRepositoryTests() : base(new ContainerizedMysqlDatabase(), nameof(SubscribedServerRepositoryTests))
        { }

        [Fact]
        public async Task Exists_WhenNoServerSubscribed_ShouldReturnFalse()
        {
            ISubscribedServerRepository repo = new SubscribedServerRepository(GetMysql());

            Assert.False(await repo.Exists(DefaultTestData.DefaultServer, 123u));
        }

        [Fact]
        public async Task Exists_Insert_ShouldBeGettable()
        {
            ISubscribedServerRepository repo = new SubscribedServerRepository(GetMysql());

            await repo.Add(DefaultTestData.DefaultServer, 123, 321u);

            var sub = await repo.Get(DefaultTestData.DefaultServer, 321u);

            Assert.Equal(123, sub.Port);
            Assert.Equal(321u, sub.ChannelId);
            Assert.Equal(DefaultTestData.DefaultServer.ServerIp, sub.Server.ServerIp);
        }

        [Fact]
        public async Task Exists_AfterInsert_ShouldExists()
        {
            ISubscribedServerRepository repo = new SubscribedServerRepository(GetMysql());

            await repo.Add(DefaultTestData.DefaultServer, 123, 321u);

            Assert.True(await repo.Exists(DefaultTestData.DefaultServer, 321u));
        }

        [Fact]
        public async Task Remove_ShouldRemoveRecord()
        {
            ISubscribedServerRepository repo = new SubscribedServerRepository(GetMysql());

            await repo.Add(DefaultTestData.DefaultServer, 123, 321u);
            await repo.Remove(DefaultTestData.DefaultServer, 321u);

            Assert.False(await repo.Exists(DefaultTestData.DefaultServer, 321u));
        }
    }
}
