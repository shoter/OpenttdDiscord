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
        [Fact]
        public async Task GetAll_ShouldGetAllServers()
        {
            ISubscribedServerRepository repo = new SubscribedServerRepository(GetMysql());
            IServerRepository serverRepo = new ServerRepository(GetMysql());

            async Task<SubscribedServer> AddServer(SubscribedServer s)
            {
                var server = await serverRepo.AddServer(s.Server.GuildId, s.Server.ServerIp, s.Server.ServerPort, s.Server.ServerName);
                return await repo.Add(server, s.Port, s.ChannelId);
            }

            var ss = new SubscribedServer[]
            {
                new SubscribedServer(
                    new Server(0, 11u, "10.0.0.1", 123, "GetAll_ShouldGetAllServers"),
                    DateTimeOffset.Now, 100, null, 150
                    ),
                new SubscribedServer(
                    new Server(1, 14u, "10.0.5.1", 123, "GetAll_ShouldGetAllServers2"),
                    DateTimeOffset.Now, 100, null, 150
                    ),
            };

            var toCheck = new List<SubscribedServer>();

            foreach (var s in ss) toCheck.Add(await AddServer(s));
            var servers = await repo.GetAll();

            foreach(var s in toCheck)
            {
                Assert.NotNull(servers.Single(x => x.ChannelId == s.ChannelId && x.Server.ServerName == s.Server.ServerName && x.Server.ServerIp == s.Server.ServerIp));
            }
        }

        [Fact]
        public async Task GetAll_ShouldGetServersFromSpecificGuild()
        {
            ISubscribedServerRepository repo = new SubscribedServerRepository(GetMysql());
            IServerRepository serverRepo = new ServerRepository(GetMysql());

            async Task<SubscribedServer> AddServer(SubscribedServer s)
            {
                var server = await serverRepo.AddServer(s.Server.GuildId, s.Server.ServerIp, s.Server.ServerPort, s.Server.ServerName);
                return await repo.Add(server, s.Port, s.ChannelId);
            }

            var ss = new SubscribedServer[]
            {
                new SubscribedServer(
                    new Server(0, 11u, "10.0.0.1", 123, "GetAll_ShouldGetServersFromSpecificGuild"),
                    DateTimeOffset.Now, 100, null, 150
                    ),
                new SubscribedServer(
                    new Server(1, 11u, "10.0.5.1", 123, "GetAll_ShouldGetServersFromSpecificGuild2"),
                    DateTimeOffset.Now, 100, null, 150
                    ),
            };

           var others = new SubscribedServer[]
           {
                new SubscribedServer(
                    new Server(0, 15u, "10.0.0.1", 123, "GetAll_ShouldGetServersFromSpecificGuild"),
                    DateTimeOffset.Now, 100, null, 150
                    ),
                new SubscribedServer(
                    new Server(1, 18u, "10.0.5.1", 123, "GetAll_ShouldGetServersFromSpecificGuild2"),
                    DateTimeOffset.Now, 100, null, 150
                    ),
           };

            var toCheck = new List<SubscribedServer>();

            foreach (var s in ss) toCheck.Add(await AddServer(s));
            foreach (var s in others) await AddServer(s);


            var servers = await repo.GetAll(11u);

            foreach (var s in toCheck)
            {
                Assert.NotNull(servers.Single(x => x.ChannelId == s.ChannelId && x.Server.Id == s.Server.Id && x.Server.ServerIp == s.Server.ServerIp));
            }
        }

    }
}
