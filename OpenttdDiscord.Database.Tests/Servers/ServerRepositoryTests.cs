using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Servers
{
    public class ServerRepositoryTests : MysqlTest
    {
        [Fact]
        public async Task InsertTest_ShouldBeAbleToGetItAfterThat()
        {
            IServerRepository repo = new ServerRepository(GetMysql());
            await repo.AddServer("192.168.0.1", 123, "testServerName");
            var server = await repo.GetServer("192.168.0.1", 123);

            Assert.Equal("192.168.0.1", server.ServerIp);
            Assert.Equal(123, server.ServerPort);
            Assert.Equal("testServerName", server.ServerName);
        }

        [Fact]
        public async Task GetAllShouldGetAllServers()
        {
            Server[] servers = new Server[]
            {
                new Server(0, "127.0.0.1", 123, "test"),
                new Server(0, "24.1.2.1", 123, "test2"),
                new Server(0, "17.1.0.1", 22, "test3"),
                new Server(0, "12.0.2.1", 33, "wara4"),
                new Server(0, "12.0.2.1", 11, "wara"),
            };
            IServerRepository repo = new ServerRepository(GetMysql());
            foreach (var s in servers)
                await repo.AddServer(s.ServerIp, s.ServerPort, s.ServerName);

            var response = await repo.GetAll();

            foreach(var s in response)
            {
                Assert.NotNull(servers.Single(x => x.ServerIp == s.ServerIp && x.ServerName == s.ServerName && x.ServerPort == s.ServerPort));
            }

        }

    }
}
